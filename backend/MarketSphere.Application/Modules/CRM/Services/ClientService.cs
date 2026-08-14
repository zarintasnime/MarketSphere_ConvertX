using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Entities.CRM;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.CRM.Services;

public sealed class ClientService : IClientService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _clock;

    public ClientService(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider clock)
    {
        _db = db;
        _currentUser = currentUser;
        _clock = clock;
    }

    public Task<PagedResult<ClientListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Clients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.ClientCode.Contains(search) || x.ClientName.Contains(search) || (x.Phone != null && x.Phone.Contains(search)));
        }

        var projected = query.OrderBy(x => x.ClientName).Select(x => new ClientListDto(
            x.ClientID, x.ClientCode, x.ClientName, x.ClientType, x.Channel, x.Phone,
            x.LifecycleStatus, x.RiskStatus, x.IsActive));
        return CrmServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<ClientDetailsDto> GetByIdAsync(int clientID, CancellationToken cancellationToken = default)
    {
        CrmServiceHelper.ValidatePositiveId(clientID, nameof(clientID));
        return await _db.Clients.AsNoTracking().Where(x => x.ClientID == clientID)
            .Select(x => new ClientDetailsDto(
                x.ClientID, x.ClientCode, x.ClientName, x.ClientType, x.Channel, x.Phone, x.Email, x.Address,
                x.GPSLat, x.GPSLng, x.RegionID, x.AreaID, x.TerritoryID, x.LifecycleStatus, x.RiskStatus,
                x.LastOrderAt, x.IsActive,
                x.Contacts.OrderByDescending(c => c.IsPrimary).ThenBy(c => c.ContactName)
                    .Select(c => new ClientContactDto(c.ClientContactID, c.ContactName, c.Designation, c.Phone, c.Email, c.IsPrimary, c.IsActive)).ToList(),
                x.CreditProfile == null ? null : new ClientCreditProfileDto(x.CreditProfile.ClientCreditProfileID, x.CreditProfile.CreditLimit, x.CreditProfile.CreditDays, x.CreditProfile.CurrentDue, x.CreditProfile.IsBlocked, x.CreditProfile.BlockReason, x.CreditProfile.LastReviewedAt),
                x.SegmentAssignments.Where(a => a.EffectiveTo == null || a.EffectiveTo > _clock.UtcNow)
                    .OrderBy(a => a.ClientSegment.SegmentName)
                    .Select(a => new ClientSegmentAssignmentDto(a.ClientSegmentAssignmentID, a.ClientSegmentID, a.ClientSegment.SegmentCode, a.ClientSegment.SegmentName, a.AssignedAt, a.EffectiveTo)).ToList()))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Client was not found.");
    }

    public async Task<int> CreateAsync(SaveClientRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateClient(request);
        var code = request.ClientCode.NormalizeCode();
        if (await _db.Clients.AnyAsync(x => x.ClientCode == code, cancellationToken))
            throw new ConflictException(BusinessRuleMessages.DuplicateCode);
        await ValidateGeographyAsync(request.RegionID, request.AreaID, request.TerritoryID, cancellationToken);

        var entity = new Client();
        Apply(entity, request, code);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.ClientID;
    }

    public async Task UpdateAsync(int clientID, SaveClientRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateClient(request);
        var entity = await CrmServiceHelper.RequireAsync(_db.Clients.Where(x => x.ClientID == clientID), "Client", cancellationToken);
        var code = request.ClientCode.NormalizeCode();
        if (await _db.Clients.AnyAsync(x => x.ClientCode == code && x.ClientID != clientID, cancellationToken))
            throw new ConflictException(BusinessRuleMessages.DuplicateCode);
        await ValidateGeographyAsync(request.RegionID, request.AreaID, request.TerritoryID, cancellationToken);
        Apply(entity, request, code);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> AddContactAsync(int clientID, SaveClientContactRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateContact(request);
        _ = await CrmServiceHelper.RequireAsync(_db.Clients.Where(x => x.ClientID == clientID), "Client", cancellationToken);
        return await _db.ExecuteInTransactionAsync(async ct =>
        {
            if (request.IsPrimary)
            {
                var primary = await _db.ClientContacts.Where(x => x.ClientID == clientID && x.IsPrimary && x.IsActive).ToListAsync(ct);
                foreach (var item in primary) item.IsPrimary = false;
            }
            var contact = new ClientContact { ClientID = clientID };
            Apply(contact, request);
            await _db.AddAsync(contact, ct);
            await _db.SaveChangesAsync(ct);
            return contact.ClientContactID;
        }, cancellationToken);
    }

    public async Task UpdateContactAsync(int clientContactID, SaveClientContactRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateContact(request);
        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var contact = await CrmServiceHelper.RequireAsync(_db.ClientContacts.Where(x => x.ClientContactID == clientContactID), "Client contact", ct);
            if (request.IsPrimary)
            {
                var primary = await _db.ClientContacts.Where(x => x.ClientID == contact.ClientID && x.IsPrimary && x.IsActive && x.ClientContactID != clientContactID).ToListAsync(ct);
                foreach (var item in primary) item.IsPrimary = false;
            }
            Apply(contact, request);
            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
    }

    public async Task SetCreditProfileAsync(int clientID, SaveClientCreditProfileRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidationHelper.Require(request.CreditLimit >= 0, nameof(request.CreditLimit), BusinessRuleMessages.CreditCannotBeNegative);
        ValidationHelper.Require(request.CreditDays >= 0, nameof(request.CreditDays), "Credit days cannot be negative.");
        ValidationHelper.Require(request.CurrentDue >= 0, nameof(request.CurrentDue), BusinessRuleMessages.CurrentDueCannotBeNegative);
        if (request.IsBlocked) ValidationHelper.RequireNotBlank(request.BlockReason, nameof(request.BlockReason), 500);
        _ = await CrmServiceHelper.RequireAsync(_db.Clients.Where(x => x.ClientID == clientID), "Client", cancellationToken);

        var profile = await _db.ClientCreditProfiles.SingleOrDefaultAsync(x => x.ClientID == clientID, cancellationToken);
        if (profile is null)
        {
            profile = new ClientCreditProfile { ClientID = clientID };
            await _db.AddAsync(profile, cancellationToken);
        }
        profile.CreditLimit = request.CreditLimit;
        profile.CreditDays = request.CreditDays;
        profile.CurrentDue = request.CurrentDue;
        profile.IsBlocked = request.IsBlocked;
        profile.BlockReason = request.IsBlocked ? request.BlockReason.NullIfWhiteSpace() : null;
        profile.LastReviewedAt = _clock.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeLifecycleAsync(int clientID, ChangeClientLifecycleRequestDto request, CancellationToken cancellationToken = default)
    {
        var client = await CrmServiceHelper.RequireAsync(_db.Clients.Where(x => x.ClientID == clientID), "Client", cancellationToken);
        if (client.LifecycleStatus == request.LifecycleStatus) return;
        var allowed = client.LifecycleStatus switch
        {
            ClientLifecycleStatus.Prospect => request.LifecycleStatus is ClientLifecycleStatus.Active or ClientLifecycleStatus.Inactive,
            ClientLifecycleStatus.Active => request.LifecycleStatus is ClientLifecycleStatus.Inactive or ClientLifecycleStatus.Churned,
            ClientLifecycleStatus.Inactive => request.LifecycleStatus is ClientLifecycleStatus.Active or ClientLifecycleStatus.Churned or ClientLifecycleStatus.ReactivationInProgress,
            ClientLifecycleStatus.Churned => request.LifecycleStatus is ClientLifecycleStatus.ReactivationInProgress,
            ClientLifecycleStatus.ReactivationInProgress => request.LifecycleStatus is ClientLifecycleStatus.Active or ClientLifecycleStatus.Churned,
            _ => false
        };
        if (!allowed) throw new BusinessRuleException(BusinessRuleMessages.InvalidStatusTransition);
        client.LifecycleStatus = request.LifecycleStatus;
        client.IsActive = request.LifecycleStatus is ClientLifecycleStatus.Active or ClientLifecycleStatus.ReactivationInProgress;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CreateSegmentAsync(SaveClientSegmentRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidationHelper.RequireNotBlank(request.SegmentCode, nameof(request.SegmentCode), 30);
        ValidationHelper.RequireNotBlank(request.SegmentName, nameof(request.SegmentName), 100);
        var code = request.SegmentCode.NormalizeCode();
        if (await _db.ClientSegments.AnyAsync(x => x.SegmentCode == code, cancellationToken))
            throw new ConflictException(BusinessRuleMessages.DuplicateCode);
        var segment = new ClientSegment { SegmentCode = code, SegmentName = request.SegmentName.Trim(), SegmentType = request.SegmentType, Description = request.Description.NullIfWhiteSpace(), IsSystemSegment = request.IsSystemSegment, IsActive = request.IsActive };
        await _db.AddAsync(segment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return segment.ClientSegmentID;
    }

    public async Task<int> AssignSegmentAsync(int clientID, AssignClientSegmentRequestDto request, CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.RequireUserID();
        _ = await CrmServiceHelper.RequireAsync(_db.Clients.Where(x => x.ClientID == clientID), "Client", cancellationToken);
        _ = await CrmServiceHelper.RequireAsync(_db.ClientSegments.Where(x => x.ClientSegmentID == request.ClientSegmentID && x.IsActive), "Client segment", cancellationToken);
        var now = _clock.UtcNow;
        if (request.EffectiveTo.HasValue)
            ValidationHelper.Require(request.EffectiveTo > now, nameof(request.EffectiveTo), "EffectiveTo must be in the future.");
        if (await _db.ClientSegmentAssignments.AnyAsync(x => x.ClientID == clientID && x.ClientSegmentID == request.ClientSegmentID && (x.EffectiveTo == null || x.EffectiveTo > now), cancellationToken))
            throw new ConflictException("The client already has an active assignment to this segment.");
        var assignment = new ClientSegmentAssignment { ClientID = clientID, ClientSegmentID = request.ClientSegmentID, AssignedAt = now, AssignedByUserID = userID, EffectiveTo = request.EffectiveTo };
        await _db.AddAsync(assignment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return assignment.ClientSegmentAssignmentID;
    }

    public async Task EndSegmentAssignmentAsync(int clientSegmentAssignmentID, DateTime effectiveTo, CancellationToken cancellationToken = default)
    {
        var assignment = await CrmServiceHelper.RequireAsync(_db.ClientSegmentAssignments.Where(x => x.ClientSegmentAssignmentID == clientSegmentAssignmentID), "Client segment assignment", cancellationToken);
        ValidationHelper.Require(effectiveTo >= assignment.AssignedAt, nameof(effectiveTo), "EffectiveTo cannot be before AssignedAt.");
        assignment.EffectiveTo = effectiveTo;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateClient(SaveClientRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.ClientCode, nameof(request.ClientCode), 30);
        ValidationHelper.RequireNotBlank(request.ClientName, nameof(request.ClientName), 200);
        ValidationHelper.RequireNotBlank(request.Address, nameof(request.Address), 500);
        CrmServiceHelper.ValidateGps(request.GPSLat, request.GPSLng);
    }

    private async Task ValidateGeographyAsync(int? regionID, int? areaID, int? territoryID, CancellationToken cancellationToken)
    {
        CrmServiceHelper.ValidateOptionalPositiveId(regionID, nameof(regionID));
        CrmServiceHelper.ValidateOptionalPositiveId(areaID, nameof(areaID));
        CrmServiceHelper.ValidateOptionalPositiveId(territoryID, nameof(territoryID));
        if (territoryID.HasValue)
        {
            var territory = await _db.Territories.AsNoTracking().SingleOrDefaultAsync(x => x.TerritoryID == territoryID, cancellationToken) ?? throw new NotFoundException("Territory was not found.");
            var area = await _db.Areas.AsNoTracking().SingleAsync(x => x.AreaID == territory.AreaID, cancellationToken);
            ValidationHelper.Require(!areaID.HasValue || areaID == area.AreaID, nameof(areaID), BusinessRuleMessages.GeographyMismatch);
            ValidationHelper.Require(!regionID.HasValue || regionID == area.RegionID, nameof(regionID), BusinessRuleMessages.GeographyMismatch);
        }
        else if (areaID.HasValue)
        {
            var area = await _db.Areas.AsNoTracking().SingleOrDefaultAsync(x => x.AreaID == areaID, cancellationToken) ?? throw new NotFoundException("Area was not found.");
            ValidationHelper.Require(!regionID.HasValue || regionID == area.RegionID, nameof(regionID), BusinessRuleMessages.GeographyMismatch);
        }
    }

    private static void Apply(Client entity, SaveClientRequestDto request, string code)
    {
        entity.ClientCode = code;
        entity.ClientName = request.ClientName.Trim();
        entity.ClientType = request.ClientType;
        entity.Channel = request.Channel;
        entity.Phone = request.Phone.NullIfWhiteSpace();
        entity.Email = request.Email.NullIfWhiteSpace()?.NormalizeEmail();
        entity.Address = request.Address.Trim();
        entity.GPSLat = request.GPSLat;
        entity.GPSLng = request.GPSLng;
        entity.RegionID = request.RegionID;
        entity.AreaID = request.AreaID;
        entity.TerritoryID = request.TerritoryID;
        entity.LifecycleStatus = request.LifecycleStatus;
        entity.RiskStatus = request.RiskStatus;
        entity.IsActive = request.IsActive;
    }

    private static void ValidateContact(SaveClientContactRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.ContactName, nameof(request.ContactName), 150);
        ValidationHelper.Require(!string.IsNullOrWhiteSpace(request.Phone) || !string.IsNullOrWhiteSpace(request.Email), nameof(request.Phone), "Phone or email is required.");
    }

    private static void Apply(ClientContact entity, SaveClientContactRequestDto request)
    {
        entity.ContactName = request.ContactName.Trim();
        entity.Designation = request.Designation.NullIfWhiteSpace();
        entity.Phone = request.Phone.NullIfWhiteSpace();
        entity.Email = request.Email.NullIfWhiteSpace()?.NormalizeEmail();
        entity.IsPrimary = request.IsPrimary;
        entity.IsActive = request.IsActive;
    }
}

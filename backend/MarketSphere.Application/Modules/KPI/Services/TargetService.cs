using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Domain.Entities.KPI;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.KPI.Services;

public sealed class TargetService : ITargetService
{
    private readonly IApplicationDbContext _db;
    private readonly IKpiProjectionService _projection;

    public TargetService(IApplicationDbContext db, IKpiProjectionService projection)
    {
        _db = db;
        _projection = projection;
    }

    public Task<PagedResult<EmployeeTargetListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.EmployeeTargets.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x => x.Employee.EmployeeCode.Contains(search) || x.Employee.User!.FullName.Contains(search));
        }
        var projected = query.OrderByDescending(x => x.TargetPeriodStart)
            .Select(x => new EmployeeTargetListDto(x.EmployeeTargetID, x.EmployeeID, x.Employee.EmployeeCode, x.Employee.User != null ? x.Employee.User.FullName : x.Employee.EmployeeCode, x.TargetPeriodStart, x.TargetPeriodEnd, x.TargetType, x.TargetValue, x.Status));
        return KpiServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<EmployeeTargetDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.EmployeeTargets.AsNoTracking().Where(x => x.EmployeeTargetID == id), "Employee target", cancellationToken);
        return ToDetails(entity);
    }

    public async Task<int> CreateAsync(SaveEmployeeTargetRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(request, null, cancellationToken);
        var entity = new EmployeeTarget();
        Apply(entity, request);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.EmployeeTargetID;
    }

    public async Task UpdateAsync(int id, SaveEmployeeTargetRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.EmployeeTargets.Where(x => x.EmployeeTargetID == id), "Employee target", cancellationToken);
        if (entity.Status is EmployeeTargetStatus.Completed or EmployeeTargetStatus.Cancelled) throw new BusinessRuleException("A completed or cancelled target cannot be edited.");
        await ValidateAsync(request, id, cancellationToken);
        Apply(entity, request);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(int id, ChangeEmployeeTargetStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.EmployeeTargets.Where(x => x.EmployeeTargetID == id), "Employee target", cancellationToken);
        var allowed = entity.Status switch
        {
            EmployeeTargetStatus.Draft => request.Status is EmployeeTargetStatus.Active or EmployeeTargetStatus.Cancelled,
            EmployeeTargetStatus.Active => request.Status is EmployeeTargetStatus.Completed or EmployeeTargetStatus.Cancelled,
            _ => false
        };
        if (!allowed) throw new BusinessRuleException("The requested target status transition is not allowed.");
        entity.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<TargetProgressDto> GetProgressAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await KpiServiceHelper.RequireAsync(_db.EmployeeTargets.AsNoTracking().Where(x => x.EmployeeTargetID == id), "Employee target", cancellationToken);
        var actual = await _projection.GetActualValueAsync(entity.EmployeeID, (int)entity.TargetType, entity.TargetPeriodStart, entity.TargetPeriodEnd, entity.CampaignID, entity.SKUID, entity.ClientID, cancellationToken);
        return new TargetProgressDto(entity.EmployeeTargetID, entity.TargetValue, actual, KpiServiceHelper.Percent(actual, entity.TargetValue), entity.Status);
    }

    private async Task ValidateAsync(SaveEmployeeTargetRequestDto request, int? id, CancellationToken cancellationToken)
    {
        if (request.TargetPeriodEnd < request.TargetPeriodStart) throw new BusinessRuleException("Target period end cannot be earlier than target period start.");
        if (request.TargetValue <= 0) throw new BusinessRuleException("Target value must be greater than zero.");
        if (request.TargetAmount < 0) throw new BusinessRuleException("Target amount cannot be negative.");
        if (!await _db.Employees.AnyAsync(x => x.EmployeeID == request.EmployeeID && x.Status == EmployeeStatus.Active, cancellationToken)) throw new NotFoundException("Active employee was not found.");
        if (request.CampaignID.HasValue && !await _db.Campaigns.AnyAsync(x => x.CampaignID == request.CampaignID, cancellationToken)) throw new NotFoundException("Campaign was not found.");
        if (request.SKUID.HasValue && !await _db.SKUs.AnyAsync(x => x.SKUID == request.SKUID && x.IsActive, cancellationToken)) throw new NotFoundException("Active SKU was not found.");
        if (request.ClientID.HasValue && !await _db.Clients.AnyAsync(x => x.ClientID == request.ClientID && x.IsActive, cancellationToken)) throw new NotFoundException("Active client was not found.");
        var duplicate = await _db.EmployeeTargets.AnyAsync(x => x.EmployeeTargetID != id && x.EmployeeID == request.EmployeeID && x.TargetPeriodStart == request.TargetPeriodStart && x.TargetPeriodEnd == request.TargetPeriodEnd && x.TargetType == request.TargetType && x.CampaignID == request.CampaignID && x.SKUID == request.SKUID && x.ClientID == request.ClientID && x.Status != EmployeeTargetStatus.Cancelled, cancellationToken);
        if (duplicate) throw new ConflictException("An active target already exists for the same employee, period and scope.");
    }

    private static void Apply(EmployeeTarget entity, SaveEmployeeTargetRequestDto request)
    {
        entity.EmployeeID = request.EmployeeID;
        entity.TargetPeriodStart = DateTime.SpecifyKind(request.TargetPeriodStart, DateTimeKind.Utc);
        entity.TargetPeriodEnd = DateTime.SpecifyKind(request.TargetPeriodEnd, DateTimeKind.Utc);
        entity.TargetType = request.TargetType;
        entity.TargetValue = request.TargetValue;
        entity.TargetAmount = request.TargetAmount;
        entity.CampaignID = request.CampaignID;
        entity.SKUID = request.SKUID;
        entity.ClientID = request.ClientID;
    }

    private static EmployeeTargetDetailsDto ToDetails(EmployeeTarget x) => new(x.EmployeeTargetID, x.EmployeeID, x.TargetPeriodStart, x.TargetPeriodEnd, x.TargetType, x.TargetValue, x.TargetAmount, x.CampaignID, x.SKUID, x.ClientID, x.Status);
}

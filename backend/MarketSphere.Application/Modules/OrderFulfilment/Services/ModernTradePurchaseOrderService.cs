using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Security;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

public sealed class ModernTradePurchaseOrderService :
    IModernTradePurchaseOrderService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public ModernTradePurchaseOrderService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<ModernTradePurchaseOrderListDto>> GetAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.ModernTradePurchaseOrders.AsNoTracking();

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.UploadedByEmployeeID == employeeID);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.PONumber.Contains(search) ||
                x.Client.ClientName.Contains(search));
        }

        var projected = query
            .OrderByDescending(x => x.ReceivedDate)
            .Select(x => new ModernTradePurchaseOrderListDto(
                x.ModernTradePurchaseOrderID,
                x.PONumber,
                x.ClientID,
                x.Client.ClientName,
                x.PODate,
                x.ReceivedDate,
                x.Status,
                x.VerificationStatus,
                x.CompletenessStatus));

        return OrderFulfilmentServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<ModernTradePurchaseOrderDetailsDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var query = _db.ModernTradePurchaseOrders
            .AsNoTracking()
            .Where(x => x.ModernTradePurchaseOrderID == id);

        if (_currentUser.IsFieldUser())
        {
            var employeeID = _currentUser.RequireEmployeeID();
            query = query.Where(x => x.UploadedByEmployeeID == employeeID);
        }

        var header = await query.SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(
                "Modern-trade purchase order was not found.");

        var items = await _db.ModernTradePurchaseOrderItems
            .AsNoTracking()
            .Where(x => x.ModernTradePurchaseOrderID == id)
            .OrderBy(x => x.ModernTradePurchaseOrderItemID)
            .Select(x => new ModernTradePurchaseOrderItemDto(
                x.ModernTradePurchaseOrderItemID,
                x.ExternalItemCode,
                x.ExternalItemName,
                x.SKUID,
                x.SKU == null ? null : x.SKU.SKUCode,
                x.MappingStatus,
                x.OrderedQuantity,
                x.AgreedUnitPrice,
                x.Discount,
                x.Note))
            .ToListAsync(cancellationToken);

        return new ModernTradePurchaseOrderDetailsDto(
            header.ModernTradePurchaseOrderID,
            header.ClientID,
            header.PONumber,
            header.PODate,
            header.ReceivedDate,
            header.UploadedByEmployeeID,
            header.Status,
            header.VerificationStatus,
            header.CompletenessStatus,
            header.VerificationNote,
            header.RejectionReason,
            header.VerifiedByEmployeeID,
            header.VerifiedAt,
            header.DuplicateHash,
            header.RequestedDeliveryDate,
            items);
    }

    public async Task<int> CreateAsync(
        SaveModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var uploadedByEmployeeID = _currentUser.ResolveFieldEmployeeID(
            request.UploadedByEmployeeID);

        await ValidateRequestAsync(
            request,
            null,
            uploadedByEmployeeID,
            cancellationToken);

        var entity = new ModernTradePurchaseOrder
        {
            Status = ModernTradePurchaseOrderStatus.Draft,
            VerificationStatus = ModernTradeVerificationStatus.Pending
        };

        Apply(entity, request, uploadedByEmployeeID);

        await _db.AddAsync(entity, cancellationToken);

        foreach (var item in request.Items)
        {
            await _db.AddAsync(
                ToItem(entity, item),
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return entity.ModernTradePurchaseOrderID;
    }

    public async Task UpdateDraftAsync(
        int id,
        SaveModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.ModernTradePurchaseOrders.Where(
                x => x.ModernTradePurchaseOrderID == id),
            "Modern-trade purchase order",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(
            entity.UploadedByEmployeeID);

        if (entity.Status != ModernTradePurchaseOrderStatus.Draft)
        {
            throw new BusinessRuleException(
                "Only a draft modern-trade purchase order can be edited.");
        }

        var uploadedByEmployeeID = _currentUser.ResolveFieldEmployeeID(
            request.UploadedByEmployeeID);

        await ValidateRequestAsync(
            request,
            id,
            uploadedByEmployeeID,
            cancellationToken);

        Apply(entity, request, uploadedByEmployeeID);

        var oldItems = await _db.ModernTradePurchaseOrderItems
            .Where(x => x.ModernTradePurchaseOrderID == id)
            .ToListAsync(cancellationToken);

        foreach (var item in oldItems)
            _db.Remove(item);

        foreach (var item in request.Items)
        {
            await _db.AddAsync(
                ToItem(entity, item),
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task MapItemAsync(
        int itemID,
        MapModernTradePurchaseOrderItemRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var item = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.ModernTradePurchaseOrderItems
                .Include(x => x.ModernTradePurchaseOrder)
                .Where(x => x.ModernTradePurchaseOrderItemID == itemID),
            "Modern-trade purchase order item",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(
            item.ModernTradePurchaseOrder.UploadedByEmployeeID);

        if (item.ModernTradePurchaseOrder.Status !=
            ModernTradePurchaseOrderStatus.Draft)
        {
            throw new BusinessRuleException(
                "Only a draft modern-trade purchase order can be mapped.");
        }

        if (!await _db.SKUs.AnyAsync(
                x => x.SKUID == request.SKUID &&
                     x.IsActive &&
                     x.Product.IsActive,
                cancellationToken))
        {
            throw new NotFoundException("Active SKU was not found.");
        }

        item.SKUID = request.SKUID;
        item.MappingStatus = ItemMappingStatus.Mapped;

        await _db.SaveChangesAsync(cancellationToken);
        await RecalculateCompletenessAsync(
            item.ModernTradePurchaseOrderID,
            cancellationToken);
    }

    public async Task SubmitAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.ModernTradePurchaseOrders.Where(
                x => x.ModernTradePurchaseOrderID == id),
            "Modern-trade purchase order",
            cancellationToken);

        _currentUser.EnsureFieldRecordOwnership(
            entity.UploadedByEmployeeID);

        await RecalculateCompletenessAsync(id, cancellationToken);

        if (entity.Status != ModernTradePurchaseOrderStatus.Draft ||
            entity.CompletenessStatus !=
            ModernTradeCompletenessStatus.Complete)
        {
            throw new BusinessRuleException(
                "A complete draft modern-trade purchase order is required.");
        }

        entity.Status = ModernTradePurchaseOrderStatus.Submitted;
        entity.VerificationStatus = ModernTradeVerificationStatus.Pending;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task VerifyAsync(
        int id,
        VerifyModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.ModernTradePurchaseOrders.Where(
                x => x.ModernTradePurchaseOrderID == id),
            "Modern-trade purchase order",
            cancellationToken);

        if (entity.Status != ModernTradePurchaseOrderStatus.Submitted)
        {
            throw new BusinessRuleException(
                "Only a submitted modern-trade purchase order can be verified.");
        }

        if (!await _db.Employees.AnyAsync(
                x => x.EmployeeID == request.VerifiedByEmployeeID,
                cancellationToken))
        {
            throw new NotFoundException(
                "Verifying employee was not found.");
        }

        await RecalculateCompletenessAsync(id, cancellationToken);

        if (request.Approve)
        {
            if (entity.CompletenessStatus !=
                ModernTradeCompletenessStatus.Complete)
            {
                throw new BusinessRuleException(
                    "The modern-trade purchase order is incomplete or contains unmapped items.");
            }

            entity.Status = ModernTradePurchaseOrderStatus.Verified;
            entity.VerificationStatus =
                ModernTradeVerificationStatus.Verified;
            entity.RejectionReason = null;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                throw new BusinessRuleException(
                    "Rejection reason is required.");
            }

            entity.Status = ModernTradePurchaseOrderStatus.Rejected;
            entity.VerificationStatus =
                ModernTradeVerificationStatus.Rejected;
            entity.RejectionReason = request.RejectionReason.Trim();
        }

        entity.VerificationNote = string.IsNullOrWhiteSpace(request.Note)
            ? null
            : request.Note.Trim();
        entity.VerifiedByEmployeeID = request.VerifiedByEmployeeID;
        entity.VerifiedAt = _clock.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateRequestAsync(
        SaveModernTradePurchaseOrderRequestDto request,
        int? id,
        int uploadedByEmployeeID,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PONumber))
        {
            throw new BusinessRuleException("PO number is required.");
        }

        if (request.Items.Count == 0 ||
            request.Items.Any(x => x.OrderedQuantity <= 0))
        {
            throw new BusinessRuleException(
                "At least one positive-quantity item is required.");
        }

        if (!await _db.Clients.AnyAsync(
                x => x.ClientID == request.ClientID &&
                     x.IsActive &&
                     x.Channel == SalesChannel.ModernTrade,
                cancellationToken))
        {
            throw new BusinessRuleException(
                "An active modern-trade client is required.");
        }

        if (!await _db.Employees.AnyAsync(
                x => x.EmployeeID == uploadedByEmployeeID &&
                     x.Status == EmployeeStatus.Active,
                cancellationToken))
        {
            throw new NotFoundException(
                "Active uploading employee was not found.");
        }

        var number = request.PONumber.Trim().ToUpperInvariant();

        if (await _db.ModernTradePurchaseOrders.AnyAsync(
                x => x.ClientID == request.ClientID &&
                     x.PONumber == number &&
                     x.ModernTradePurchaseOrderID != id,
                cancellationToken))
        {
            throw new ConflictException(
                "The client PO number already exists.");
        }

        if (!string.IsNullOrWhiteSpace(request.DuplicateHash) &&
            await _db.ModernTradePurchaseOrders.AnyAsync(
                x => x.DuplicateHash == request.DuplicateHash &&
                     x.ModernTradePurchaseOrderID != id,
                cancellationToken))
        {
            throw new ConflictException(
                "A modern-trade purchase order with the same duplicate hash already exists.");
        }
    }

    private static void Apply(
        ModernTradePurchaseOrder entity,
        SaveModernTradePurchaseOrderRequestDto request,
        int uploadedByEmployeeID)
    {
        entity.ClientID = request.ClientID;
        entity.PONumber = request.PONumber.Trim().ToUpperInvariant();
        entity.PODate = request.PODate.Date;
        entity.ReceivedDate = request.ReceivedDate;
        entity.UploadedByEmployeeID = uploadedByEmployeeID;
        entity.DuplicateHash = string.IsNullOrWhiteSpace(request.DuplicateHash)
            ? null
            : request.DuplicateHash.Trim();
        entity.RequestedDeliveryDate = request.RequestedDeliveryDate?.Date;
        entity.CompletenessStatus = request.Items.All(x => x.SKUID.HasValue)
            ? ModernTradeCompletenessStatus.Complete
            : ModernTradeCompletenessStatus.Incomplete;
    }

    private static ModernTradePurchaseOrderItem ToItem(
        ModernTradePurchaseOrder header,
        SaveModernTradePurchaseOrderItemRequestDto request) =>
        new()
        {
            ModernTradePurchaseOrder = header,
            ExternalItemCode = string.IsNullOrWhiteSpace(
                request.ExternalItemCode)
                ? null
                : request.ExternalItemCode.Trim(),
            ExternalItemName = string.IsNullOrWhiteSpace(
                request.ExternalItemName)
                ? null
                : request.ExternalItemName.Trim(),
            SKUID = request.SKUID,
            MappingStatus = request.SKUID.HasValue
                ? ItemMappingStatus.Mapped
                : ItemMappingStatus.Unmapped,
            OrderedQuantity = request.OrderedQuantity,
            AgreedUnitPrice = request.AgreedUnitPrice,
            Discount = request.Discount,
            Note = string.IsNullOrWhiteSpace(request.Note)
                ? null
                : request.Note.Trim()
        };

    private async Task RecalculateCompletenessAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var header = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.ModernTradePurchaseOrders.Where(
                x => x.ModernTradePurchaseOrderID == id),
            "Modern-trade purchase order",
            cancellationToken);

        var items = await _db.ModernTradePurchaseOrderItems
            .Where(x => x.ModernTradePurchaseOrderID == id)
            .ToListAsync(cancellationToken);

        header.CompletenessStatus =
            items.Count > 0 &&
            items.All(x =>
                x.SKUID.HasValue &&
                x.MappingStatus == ItemMappingStatus.Mapped)
                ? ModernTradeCompletenessStatus.Complete
                : ModernTradeCompletenessStatus.Incomplete;

        header.VerificationStatus = header.CompletenessStatus ==
            ModernTradeCompletenessStatus.Complete
                ? ModernTradeVerificationStatus.Pending
                : items.Any(x => x.SKUID.HasValue)
                    ? ModernTradeVerificationStatus.MappingRequired
                    : ModernTradeVerificationStatus.Incomplete;

        await _db.SaveChangesAsync(cancellationToken);
    }
}

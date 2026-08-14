using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Application.Modules.Procurement.DTOs;
using MarketSphere.Application.Modules.Procurement.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Procurement.Services;

public sealed class GoodsReceiptService : IGoodsReceiptService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GoodsReceiptService(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public Task<PagedResult<GoodsReceiptListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var q = _db.GoodsReceipts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.GoodsReceiptNo.Contains(s) || x.PurchaseOrder.PurchaseOrderNo.Contains(s));
        }
        var p = q.OrderByDescending(x => x.GoodsReceiptID).Select(x => new GoodsReceiptListDto(
            x.GoodsReceiptID, x.GoodsReceiptNo, x.PurchaseOrder.PurchaseOrderNo,
            x.Warehouse.WarehouseName, x.ReceivedDate, x.Status, x.QualityCheckStatus));
        return InventoryServiceHelper.ToPagedAsync(p, request, cancellationToken);
    }

    public async Task<GoodsReceiptDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(
            _db.GoodsReceipts.AsNoTracking().Where(x => x.GoodsReceiptID == id), "Goods receipt", cancellationToken);
        var items = await _db.GoodsReceiptItems.AsNoTracking().Where(x => x.GoodsReceiptID == id)
            .OrderBy(x => x.GoodsReceiptItemID)
            .Select(x => new GoodsReceiptItemDto(x.GoodsReceiptItemID, x.PurchaseOrderItemID, x.SKUID,
                x.SKU.SKUCode, x.AcceptedQuantity, x.RejectedQuantity, x.BatchNo, x.ExpiryDate,
                x.UnitCost, x.BatchID, x.RejectionReason)).ToListAsync(cancellationToken);
        return new(e.GoodsReceiptID, e.GoodsReceiptNo, e.PurchaseOrderID, e.WarehouseID,
            e.ReceivedDate, e.ReceivedByEmployeeID, e.SupplierChallanNo, e.Status, e.QualityCheckStatus, items);
    }

    public async Task<int> CreateAsync(SaveGoodsReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(request, null, cancellationToken);
        var e = new GoodsReceipt(); Apply(e, request); await _db.AddAsync(e, cancellationToken);
        foreach (var i in request.Items) await _db.AddAsync(ToItem(e, i), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken); return e.GoodsReceiptID;
    }

    public async Task UpdateAsync(int id, SaveGoodsReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.GoodsReceipts.Where(x => x.GoodsReceiptID == id), "Goods receipt", cancellationToken);
        if (e.Status != GoodsReceiptStatus.Draft && e.Status != GoodsReceiptStatus.QualityCheck)
            throw new BusinessRuleException("Only a draft or quality-check goods receipt can be edited.");
        await ValidateAsync(request, id, cancellationToken); Apply(e, request);
        var old = await _db.GoodsReceiptItems.Where(x => x.GoodsReceiptID == id).ToListAsync(cancellationToken);
        foreach (var x in old) _db.Remove(x);
        foreach (var i in request.Items) await _db.AddAsync(ToItem(e, i), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteQualityCheckAsync(int id, CompleteQualityCheckRequestDto request, CancellationToken cancellationToken = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.GoodsReceipts.Where(x => x.GoodsReceiptID == id), "Goods receipt", cancellationToken);
        if (e.Status == GoodsReceiptStatus.Posted) throw new BusinessRuleException("A posted goods receipt cannot be changed.");
        if (request.QualityCheckStatus == QualityCheckStatus.Pending) throw new BusinessRuleException("A final quality-check result is required.");
        var hasAccepted = await _db.GoodsReceiptItems.AnyAsync(x => x.GoodsReceiptID == id && x.AcceptedQuantity > 0, cancellationToken);
        var hasRejected = await _db.GoodsReceiptItems.AnyAsync(x => x.GoodsReceiptID == id && x.RejectedQuantity > 0, cancellationToken);
        e.QualityCheckStatus = request.QualityCheckStatus;
        e.Status = request.QualityCheckStatus == QualityCheckStatus.Failed || !hasAccepted
            ? GoodsReceiptStatus.Rejected
            : GoodsReceiptStatus.Approved;
        if (request.QualityCheckStatus == QualityCheckStatus.PartiallyAccepted && !hasRejected)
            throw new BusinessRuleException("Partially accepted quality check requires a rejected quantity.");
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task PostAsync(int id, PostGoodsReceiptRequestDto request, CancellationToken cancellationToken = default)
    {
        var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
        await _db.ExecuteInTransactionAsync(async ct =>
        {
            var e = await _db.GoodsReceipts.Include(x => x.Items).ThenInclude(x => x.SKU).ThenInclude(x => x.Product)
                .Include(x => x.PurchaseOrder).ThenInclude(x => x.Items)
                .SingleOrDefaultAsync(x => x.GoodsReceiptID == id, ct)
                ?? throw new NotFoundException("Goods receipt was not found.");
            if (e.Status != GoodsReceiptStatus.Approved)
                throw new BusinessRuleException("Only an approved goods receipt can be posted.");

            foreach (var item in e.Items.Where(x => x.AcceptedQuantity > 0))
            {
                var batchID = await InventoryServiceHelper.GetOrCreateBatchAsync(_db, item.SKUID, item.BatchNo,
                    item.ManufacturingDate, item.ExpiryDate, item.UnitCost, ct);
                item.BatchID = batchID;
                await InventoryServiceHelper.PostMovementAsync(_db, e.WarehouseID, item.SKUID, batchID,
                    StockMovementType.GoodsReceipt, item.AcceptedQuantity, 0,
                    ReferenceTypeCodes.GoodsReceipt, e.GoodsReceiptID, userID, request.Note, ct);
                var poItem = e.PurchaseOrder.Items.Single(x => x.PurchaseOrderItemID == item.PurchaseOrderItemID);
                var newReceived = poItem.ReceivedQuantity + item.AcceptedQuantity;
                if (newReceived > poItem.OrderedQuantity)
                    throw new BusinessRuleException("Received quantity exceeds the purchase-order quantity.");
                poItem.ReceivedQuantity = newReceived;
            }
            e.Status = GoodsReceiptStatus.Posted;
            e.PurchaseOrder.Status = e.PurchaseOrder.Items.All(x => x.ReceivedQuantity >= x.OrderedQuantity)
                ? PurchaseOrderStatus.Received : PurchaseOrderStatus.PartiallyReceived;
            await _db.SaveChangesAsync(ct); return true;
        }, cancellationToken);
    }

    private async Task ValidateAsync(SaveGoodsReceiptRequestDto r, int? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.GoodsReceiptNo)) throw new BusinessRuleException("Goods receipt number is required.");
        if (r.Items.Count == 0) throw new BusinessRuleException("At least one goods-receipt item is required.");
        if (r.Items.Any(x => x.AcceptedQuantity < 0 || x.RejectedQuantity < 0 || x.AcceptedQuantity + x.RejectedQuantity <= 0 || x.UnitCost < 0))
            throw new BusinessRuleException("Goods-receipt quantities or cost are invalid.");
        if (r.Items.Any(x => x.RejectedQuantity > 0 && string.IsNullOrWhiteSpace(x.RejectionReason)))
            throw new BusinessRuleException("A rejection reason is required for rejected quantity.");
        if (r.Items.GroupBy(x => x.PurchaseOrderItemID).Any(x => x.Count() > 1))
            throw new BusinessRuleException("Duplicate purchase-order items are not allowed.");
        var no = r.GoodsReceiptNo.Trim().ToUpperInvariant();
        if (await _db.GoodsReceipts.AnyAsync(x => x.GoodsReceiptNo == no && x.GoodsReceiptID != id, ct))
            throw new ConflictException("Goods receipt number already exists.");
        var po = await _db.PurchaseOrders.Include(x => x.Items).SingleOrDefaultAsync(x => x.PurchaseOrderID == r.PurchaseOrderID, ct)
            ?? throw new NotFoundException("Purchase order was not found.");
        if (po.Status != PurchaseOrderStatus.Approved && po.Status != PurchaseOrderStatus.PartiallyReceived)
            throw new BusinessRuleException("An approved or partially received purchase order is required.");
        if (!await _db.Warehouses.AnyAsync(x => x.WarehouseID == r.WarehouseID && x.IsActive, ct))
            throw new BusinessRuleException("An active warehouse is required.");
        if (!await _db.Employees.AnyAsync(x => x.EmployeeID == r.ReceivedByEmployeeID, ct))
            throw new BusinessRuleException("Receiving employee was not found.");
        foreach (var item in r.Items)
        {
            var poItem = po.Items.SingleOrDefault(x => x.PurchaseOrderItemID == item.PurchaseOrderItemID && x.SKUID == item.SKUID)
                ?? throw new BusinessRuleException("The goods-receipt item does not belong to the purchase order.");
            if (poItem.ReceivedQuantity + item.AcceptedQuantity > poItem.OrderedQuantity)
                throw new BusinessRuleException("Received quantity exceeds the purchase-order quantity.");
        }
    }
    private static void Apply(GoodsReceipt e, SaveGoodsReceiptRequestDto r) { e.GoodsReceiptNo = r.GoodsReceiptNo.Trim().ToUpperInvariant(); e.PurchaseOrderID = r.PurchaseOrderID; e.WarehouseID = r.WarehouseID; e.ReceivedDate = r.ReceivedDate.Date; e.ReceivedByEmployeeID = r.ReceivedByEmployeeID; e.SupplierChallanNo = r.SupplierChallanNo?.Trim(); }
    private static GoodsReceiptItem ToItem(GoodsReceipt e, GoodsReceiptItemInputDto i) => new() { GoodsReceipt = e, PurchaseOrderItemID = i.PurchaseOrderItemID, SKUID = i.SKUID, AcceptedQuantity = i.AcceptedQuantity, RejectedQuantity = i.RejectedQuantity, BatchNo = i.BatchNo?.Trim().ToUpperInvariant(), ManufacturingDate = i.ManufacturingDate?.Date, ExpiryDate = i.ExpiryDate?.Date, UnitCost = i.UnitCost, RejectionReason = i.RejectionReason?.Trim() };
}

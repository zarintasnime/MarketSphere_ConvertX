using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Application.Modules.Procurement.DTOs;
using MarketSphere.Application.Modules.Procurement.Interfaces;
using MarketSphere.Domain.Entities.Procurement;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Procurement.Services;

public sealed class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IApplicationDbContext _db;
    public PurchaseOrderService(IApplicationDbContext db) => _db = db;

    public Task<PagedResult<PurchaseOrderListDto>> GetAsync(PagedRequest r, CancellationToken ct = default)
    {
        var q = _db.PurchaseOrders.AsNoTracking(); if (!string.IsNullOrWhiteSpace(r.Search)) { var s = r.Search.Trim(); q = q.Where(x => x.PurchaseOrderNo.Contains(s) || x.Supplier.SupplierName.Contains(s)); }
        return InventoryServiceHelper.ToPagedAsync(q.OrderByDescending(x => x.PurchaseOrderID).Select(x => new PurchaseOrderListDto(x.PurchaseOrderID, x.PurchaseOrderNo, x.Supplier.SupplierName, x.OrderDate, x.ExpectedDeliveryDate, x.Status, x.NetAmount)), r, ct);
    }

    public async Task<PurchaseOrderDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseOrders.AsNoTracking().Where(x => x.PurchaseOrderID == id), "Purchase order", ct);
        var items = await _db.PurchaseOrderItems.AsNoTracking().Where(x => x.PurchaseOrderID == id).OrderBy(x => x.PurchaseOrderItemID).Select(x => new PurchaseOrderItemDto(x.PurchaseOrderItemID, x.SKUID, x.SKU.SKUCode, x.SKU.SKUName, x.OrderedQuantity, x.ReceivedQuantity, x.UnitCost, x.DiscountAmount, x.TaxAmount, x.LineTotal)).ToListAsync(ct);
        return new(e.PurchaseOrderID, e.PurchaseOrderNo, e.SupplierID, e.PurchaseRequisitionID, e.BranchID, e.OrderDate, e.ExpectedDeliveryDate, e.Status, e.GrossAmount, e.DiscountAmount, e.TaxAmount, e.NetAmount, items);
    }

    public async Task<int> CreateAsync(SavePurchaseOrderRequestDto r, CancellationToken ct = default)
    { await ValidateAsync(r, null, ct); var e = new PurchaseOrder(); Apply(e, r); await _db.AddAsync(e, ct); foreach (var i in r.Items) await _db.AddAsync(ToItem(e, i), ct); await _db.SaveChangesAsync(ct); return e.PurchaseOrderID; }

    public async Task UpdateAsync(int id, SavePurchaseOrderRequestDto r, CancellationToken ct = default)
    { var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseOrders.Where(x => x.PurchaseOrderID == id), "Purchase order", ct); if (e.Status != PurchaseOrderStatus.Draft) throw new BusinessRuleException("Only a draft purchase order can be edited."); await ValidateAsync(r, id, ct); Apply(e, r); var old = await _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToListAsync(ct); foreach (var x in old) _db.Remove(x); foreach (var i in r.Items) await _db.AddAsync(ToItem(e, i), ct); await _db.SaveChangesAsync(ct); }

    public async Task ChangeStatusAsync(int id, ChangePurchaseOrderStatusRequestDto r, CancellationToken ct = default)
    { var e = await InventoryServiceHelper.RequireAsync(_db.PurchaseOrders.Where(x => x.PurchaseOrderID == id), "Purchase order", ct); var ok = (e.Status, r.Status) switch { (PurchaseOrderStatus.Draft, PurchaseOrderStatus.Submitted) => true, (PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Approved) => true, (PurchaseOrderStatus.Submitted, PurchaseOrderStatus.Cancelled) => true, (PurchaseOrderStatus.Approved, PurchaseOrderStatus.Cancelled) => true, (PurchaseOrderStatus.Received, PurchaseOrderStatus.Closed) => true, _ => false }; if (!ok) throw new BusinessRuleException("The requested status transition is not allowed."); e.Status = r.Status; await _db.SaveChangesAsync(ct); }

    private async Task ValidateAsync(SavePurchaseOrderRequestDto r, int? id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.PurchaseOrderNo)) throw new BusinessRuleException("Purchase order number is required."); InventoryServiceHelper.EnsureDistinctPositive(r.Items, x => x.SKUID, x => x.OrderedQuantity); if (r.Items.Any(x => x.UnitCost < 0 || x.DiscountAmount < 0 || x.TaxAmount < 0)) throw new BusinessRuleException("Purchase-order amounts cannot be negative.");
        var no = r.PurchaseOrderNo.Trim().ToUpperInvariant(); if (await _db.PurchaseOrders.AnyAsync(x => x.PurchaseOrderNo == no && x.PurchaseOrderID != id, ct)) throw new ConflictException("Purchase order number already exists."); if (!await _db.Suppliers.AnyAsync(x => x.SupplierID == r.SupplierID && x.Status == SupplierStatus.Active, ct)) throw new BusinessRuleException("An active supplier is required."); if (!await _db.Branches.AnyAsync(x => x.BranchID == r.BranchID && x.IsActive, ct)) throw new BusinessRuleException("An active branch is required.");
        if (r.PurchaseRequisitionID.HasValue && !await _db.PurchaseRequisitions.AnyAsync(x => x.PurchaseRequisitionID == r.PurchaseRequisitionID && x.Status == PurchaseRequisitionStatus.Approved, ct)) throw new BusinessRuleException("An approved purchase requisition is required."); var ids = r.Items.Select(x => x.SKUID).ToArray(); if (await _db.SupplierProducts.CountAsync(x => x.SupplierID == r.SupplierID && ids.Contains(x.SKUID) && x.IsActive, ct) != ids.Length) throw new BusinessRuleException("Every SKU must be enabled for the selected supplier.");
    }
    private static void Apply(PurchaseOrder e, SavePurchaseOrderRequestDto r) { e.PurchaseOrderNo = r.PurchaseOrderNo.Trim().ToUpperInvariant(); e.SupplierID = r.SupplierID; e.PurchaseRequisitionID = r.PurchaseRequisitionID; e.BranchID = r.BranchID; e.OrderDate = r.OrderDate.Date; e.ExpectedDeliveryDate = r.ExpectedDeliveryDate?.Date; e.GrossAmount = r.Items.Sum(x => x.OrderedQuantity * x.UnitCost); e.DiscountAmount = r.Items.Sum(x => x.DiscountAmount); e.TaxAmount = r.Items.Sum(x => x.TaxAmount); e.NetAmount = e.GrossAmount - e.DiscountAmount + e.TaxAmount; }
    private static PurchaseOrderItem ToItem(PurchaseOrder e, PurchaseOrderItemInputDto i) { var line = i.OrderedQuantity * i.UnitCost - i.DiscountAmount + i.TaxAmount; return new() { PurchaseOrder = e, SKUID = i.SKUID, OrderedQuantity = i.OrderedQuantity, UnitCost = i.UnitCost, DiscountAmount = i.DiscountAmount, TaxAmount = i.TaxAmount, LineTotal = line }; }
}

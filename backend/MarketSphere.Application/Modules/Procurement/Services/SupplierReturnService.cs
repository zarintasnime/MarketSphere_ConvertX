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

public sealed class SupplierReturnService : ISupplierReturnService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SupplierReturnService(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public Task<PagedResult<SupplierReturnListDto>> GetAsync(PagedRequest r, CancellationToken ct = default)
    { var q = _db.SupplierReturns.AsNoTracking(); if (!string.IsNullOrWhiteSpace(r.Search)) { var s = r.Search.Trim(); q = q.Where(x => x.SupplierReturnNo.Contains(s) || x.Supplier.SupplierName.Contains(s)); } return InventoryServiceHelper.ToPagedAsync(q.OrderByDescending(x => x.SupplierReturnID).Select(x => new SupplierReturnListDto(x.SupplierReturnID, x.SupplierReturnNo, x.Supplier.SupplierName, x.Warehouse.WarehouseName, x.ReturnDate, x.Status)), r, ct); }

    public async Task<SupplierReturnDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    { var e = await InventoryServiceHelper.RequireAsync(_db.SupplierReturns.AsNoTracking().Where(x => x.SupplierReturnID == id), "Supplier return", ct); var items = await _db.SupplierReturnItems.AsNoTracking().Where(x => x.SupplierReturnID == id).Select(x => new SupplierReturnItemDto(x.SupplierReturnItemID, x.SKUID, x.SKU.SKUCode, x.BatchID, x.Quantity, x.UnitCost, x.Reason, x.StockMovementID)).ToListAsync(ct); return new(e.SupplierReturnID, e.SupplierReturnNo, e.SupplierID, e.GoodsReceiptID, e.WarehouseID, e.ReturnDate, e.Reason, e.Status, items); }

    public async Task<int> CreateAsync(SaveSupplierReturnRequestDto r, CancellationToken ct = default) { await ValidateAsync(r, null, ct); var e = new SupplierReturn(); Apply(e, r); await _db.AddAsync(e, ct); foreach (var i in r.Items) await _db.AddAsync(ToItem(e, i), ct); await _db.SaveChangesAsync(ct); return e.SupplierReturnID; }
    public async Task UpdateAsync(int id, SaveSupplierReturnRequestDto r, CancellationToken ct = default) { var e = await InventoryServiceHelper.RequireAsync(_db.SupplierReturns.Where(x => x.SupplierReturnID == id), "Supplier return", ct); if (e.Status != SupplierReturnStatus.Draft) throw new BusinessRuleException("Only a draft supplier return can be edited."); await ValidateAsync(r, id, ct); Apply(e, r); var old = await _db.SupplierReturnItems.Where(x => x.SupplierReturnID == id).ToListAsync(ct); foreach (var x in old) _db.Remove(x); foreach (var i in r.Items) await _db.AddAsync(ToItem(e, i), ct); await _db.SaveChangesAsync(ct); }
    public async Task ChangeStatusAsync(int id, ChangeSupplierReturnStatusRequestDto r, CancellationToken ct = default) { var e = await InventoryServiceHelper.RequireAsync(_db.SupplierReturns.Where(x => x.SupplierReturnID == id), "Supplier return", ct); var ok = (e.Status, r.Status) switch { (SupplierReturnStatus.Draft, SupplierReturnStatus.Submitted) => true, (SupplierReturnStatus.Submitted, SupplierReturnStatus.Approved) => true, (SupplierReturnStatus.Submitted, SupplierReturnStatus.Cancelled) => true, (SupplierReturnStatus.Draft, SupplierReturnStatus.Cancelled) => true, _ => false }; if (!ok) throw new BusinessRuleException("The requested status transition is not allowed."); e.Status = r.Status; await _db.SaveChangesAsync(ct); }

    public async Task PostAsync(int id, PostSupplierReturnRequestDto r, CancellationToken ct = default)
    {
        var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
        await _db.ExecuteInTransactionAsync(async token =>
        { var e = await _db.SupplierReturns.Include(x => x.Items).SingleOrDefaultAsync(x => x.SupplierReturnID == id, token) ?? throw new NotFoundException("Supplier return was not found."); if (e.Status != SupplierReturnStatus.Approved) throw new BusinessRuleException("Only an approved supplier return can be posted."); foreach (var i in e.Items) { var m = await InventoryServiceHelper.PostMovementAsync(_db, e.WarehouseID, i.SKUID, i.BatchID, StockMovementType.SupplierReturn, 0, i.Quantity, ReferenceTypeCodes.SupplierReturn, e.SupplierReturnID, userID, r.Note, token); i.StockMovementID = m.StockMovementID; } e.Status = SupplierReturnStatus.Posted; await _db.SaveChangesAsync(token); return true; }, ct);
    }

    private async Task ValidateAsync(SaveSupplierReturnRequestDto r, int? id, CancellationToken ct)
    { if (string.IsNullOrWhiteSpace(r.SupplierReturnNo) || string.IsNullOrWhiteSpace(r.Reason)) throw new BusinessRuleException("Return number and reason are required."); InventoryServiceHelper.EnsureDistinctPositive(r.Items, x => HashCode.Combine(x.SKUID, x.BatchID), x => x.Quantity); if (r.Items.Any(x => x.UnitCost < 0 || string.IsNullOrWhiteSpace(x.Reason))) throw new BusinessRuleException("Supplier-return item values are invalid."); var no = r.SupplierReturnNo.Trim().ToUpperInvariant(); if (await _db.SupplierReturns.AnyAsync(x => x.SupplierReturnNo == no && x.SupplierReturnID != id, ct)) throw new ConflictException("Supplier return number already exists."); if (!await _db.Suppliers.AnyAsync(x => x.SupplierID == r.SupplierID, ct)) throw new BusinessRuleException("Supplier was not found."); if (!await _db.Warehouses.AnyAsync(x => x.WarehouseID == r.WarehouseID && x.IsActive, ct)) throw new BusinessRuleException("An active warehouse is required."); if (r.GoodsReceiptID.HasValue && !await _db.GoodsReceipts.AnyAsync(x => x.GoodsReceiptID == r.GoodsReceiptID && x.Status == GoodsReceiptStatus.Posted, ct)) throw new BusinessRuleException("A posted goods receipt is required."); foreach (var i in r.Items) { if (i.BatchID.HasValue && !await _db.Batches.AnyAsync(x => x.BatchID == i.BatchID && x.SKUID == i.SKUID, ct)) throw new BusinessRuleException("Batch does not belong to the selected SKU."); } }
    private static void Apply(SupplierReturn e, SaveSupplierReturnRequestDto r) { e.SupplierReturnNo = r.SupplierReturnNo.Trim().ToUpperInvariant(); e.SupplierID = r.SupplierID; e.GoodsReceiptID = r.GoodsReceiptID; e.WarehouseID = r.WarehouseID; e.ReturnDate = r.ReturnDate.Date; e.Reason = r.Reason.Trim(); }
    private static SupplierReturnItem ToItem(SupplierReturn e, SupplierReturnItemInputDto i) => new() { SupplierReturn = e, SKUID = i.SKUID, BatchID = i.BatchID, Quantity = i.Quantity, UnitCost = i.UnitCost, Reason = i.Reason.Trim() };
}

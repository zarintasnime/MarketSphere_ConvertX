using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.DTOs;
using MarketSphere.Application.Modules.Inventory.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Inventory.Services;

public sealed class StockAdjustmentService : IStockAdjustmentService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StockAdjustmentService(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public Task<PagedResult<StockAdjustmentListDto>> GetAsync(PagedRequest r, CancellationToken ct = default)
    { var q = _db.StockAdjustments.AsNoTracking(); if (!string.IsNullOrWhiteSpace(r.Search)) { var s = r.Search.Trim(); q = q.Where(x => x.StockAdjustmentNo.Contains(s) || x.Reason.Contains(s)); } return InventoryServiceHelper.ToPagedAsync(q.OrderByDescending(x => x.StockAdjustmentID).Select(x => new StockAdjustmentListDto(x.StockAdjustmentID, x.StockAdjustmentNo, x.Warehouse.WarehouseName, x.AdjustmentDate, x.Reason, x.Status)), r, ct); }

    public async Task<StockAdjustmentDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    { var e = await InventoryServiceHelper.RequireAsync(_db.StockAdjustments.AsNoTracking().Where(x => x.StockAdjustmentID == id), "Stock adjustment", ct); var items = await _db.StockAdjustmentItems.AsNoTracking().Where(x => x.StockAdjustmentID == id).Select(x => new StockAdjustmentItemDto(x.StockAdjustmentItemID, x.SKUID, x.SKU.SKUCode, x.BatchID, x.AdjustmentQuantity, x.UnitCost, x.Note, x.StockMovementID)).ToListAsync(ct); return new(e.StockAdjustmentID, e.StockAdjustmentNo, e.WarehouseID, e.AdjustmentDate, e.Reason, e.Status, e.PerformedByEmployeeID, items); }

    public async Task<int> CreateAsync(SaveStockAdjustmentRequestDto r, CancellationToken ct = default) { await ValidateAsync(r, null, ct); var e = new StockAdjustment(); Apply(e, r); await _db.AddAsync(e, ct); foreach (var i in r.Items) await _db.AddAsync(ToItem(e, i), ct); await _db.SaveChangesAsync(ct); return e.StockAdjustmentID; }
    public async Task UpdateAsync(int id, SaveStockAdjustmentRequestDto r, CancellationToken ct = default) { var e = await InventoryServiceHelper.RequireAsync(_db.StockAdjustments.Where(x => x.StockAdjustmentID == id), "Stock adjustment", ct); if (e.Status != StockAdjustmentStatus.Draft) throw new BusinessRuleException("Only a draft stock adjustment can be edited."); await ValidateAsync(r, id, ct); Apply(e, r); var old = await _db.StockAdjustmentItems.Where(x => x.StockAdjustmentID == id).ToListAsync(ct); foreach (var x in old) _db.Remove(x); foreach (var i in r.Items) await _db.AddAsync(ToItem(e, i), ct); await _db.SaveChangesAsync(ct); }
    public async Task ChangeStatusAsync(int id, ChangeStockAdjustmentStatusRequestDto r, CancellationToken ct = default) { var e = await InventoryServiceHelper.RequireAsync(_db.StockAdjustments.Where(x => x.StockAdjustmentID == id), "Stock adjustment", ct); var ok = (e.Status, r.Status) switch { (StockAdjustmentStatus.Draft, StockAdjustmentStatus.Submitted) => true, (StockAdjustmentStatus.Submitted, StockAdjustmentStatus.Approved) => true, (StockAdjustmentStatus.Submitted, StockAdjustmentStatus.Rejected) => true, (StockAdjustmentStatus.Draft, StockAdjustmentStatus.Cancelled) => true, _ => false }; if (!ok) throw new BusinessRuleException("The requested status transition is not allowed."); e.Status = r.Status; await _db.SaveChangesAsync(ct); }

    public async Task PostAsync(int id, PostStockAdjustmentRequestDto r, CancellationToken ct = default)
    { var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required."); await _db.ExecuteInTransactionAsync(async token => { var e = await _db.StockAdjustments.Include(x => x.Items).SingleOrDefaultAsync(x => x.StockAdjustmentID == id, token) ?? throw new NotFoundException("Stock adjustment was not found."); if (e.Status != StockAdjustmentStatus.Approved) throw new BusinessRuleException("Only an approved stock adjustment can be posted."); foreach (var i in e.Items) { var movement = await InventoryServiceHelper.PostMovementAsync(_db, e.WarehouseID, i.SKUID, i.BatchID, i.AdjustmentQuantity > 0 ? StockMovementType.AdjustmentIn : StockMovementType.AdjustmentOut, i.AdjustmentQuantity > 0 ? i.AdjustmentQuantity : 0, i.AdjustmentQuantity < 0 ? Math.Abs(i.AdjustmentQuantity) : 0, ReferenceTypeCodes.StockAdjustment, e.StockAdjustmentID, userID, r.Note, token); i.StockMovementID = movement.StockMovementID; } e.Status = StockAdjustmentStatus.Posted; await _db.SaveChangesAsync(token); return true; }, ct); }

    private async Task ValidateAsync(SaveStockAdjustmentRequestDto r, int? id, CancellationToken ct) { if (string.IsNullOrWhiteSpace(r.StockAdjustmentNo) || string.IsNullOrWhiteSpace(r.Reason)) throw new BusinessRuleException("Adjustment number and reason are required."); if (r.Items.Count == 0 || r.Items.Any(x => x.AdjustmentQuantity == 0 || x.UnitCost < 0) || r.Items.GroupBy(x => HashCode.Combine(x.SKUID, x.BatchID)).Any(x => x.Count() > 1)) throw new BusinessRuleException("Stock-adjustment items are invalid."); var no = r.StockAdjustmentNo.Trim().ToUpperInvariant(); if (await _db.StockAdjustments.AnyAsync(x => x.StockAdjustmentNo == no && x.StockAdjustmentID != id, ct)) throw new ConflictException("Stock adjustment number already exists."); if (!await _db.Warehouses.AnyAsync(x => x.WarehouseID == r.WarehouseID && x.IsActive, ct)) throw new BusinessRuleException("An active warehouse is required."); if (!await _db.Employees.AnyAsync(x => x.EmployeeID == r.PerformedByEmployeeID, ct)) throw new BusinessRuleException("Performing employee was not found."); foreach (var i in r.Items) { if (i.BatchID.HasValue && !await _db.Batches.AnyAsync(x => x.BatchID == i.BatchID && x.SKUID == i.SKUID, ct)) throw new BusinessRuleException("Batch does not belong to the selected SKU."); } }
    private static void Apply(StockAdjustment e, SaveStockAdjustmentRequestDto r) { e.StockAdjustmentNo = r.StockAdjustmentNo.Trim().ToUpperInvariant(); e.WarehouseID = r.WarehouseID; e.AdjustmentDate = r.AdjustmentDate.Date; e.Reason = r.Reason.Trim(); e.PerformedByEmployeeID = r.PerformedByEmployeeID; }
    private static StockAdjustmentItem ToItem(StockAdjustment e, StockAdjustmentItemInputDto i) => new() { StockAdjustment = e, SKUID = i.SKUID, BatchID = i.BatchID, AdjustmentQuantity = i.AdjustmentQuantity, UnitCost = i.UnitCost, Note = i.Note?.Trim() };
}

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

public sealed class StockTransferService : IStockTransferService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StockTransferService(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public Task<PagedResult<StockTransferListDto>> GetAsync(PagedRequest r, CancellationToken ct = default)
    { var q = _db.StockTransfers.AsNoTracking(); if (!string.IsNullOrWhiteSpace(r.Search)) { var s = r.Search.Trim(); q = q.Where(x => x.StockTransferNo.Contains(s) || x.FromWarehouse.WarehouseName.Contains(s) || x.ToWarehouse.WarehouseName.Contains(s)); } return InventoryServiceHelper.ToPagedAsync(q.OrderByDescending(x => x.StockTransferID).Select(x => new StockTransferListDto(x.StockTransferID, x.StockTransferNo, x.FromWarehouse.WarehouseName, x.ToWarehouse.WarehouseName, x.RequestedAt, x.Status)), r, ct); }

    public async Task<StockTransferDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    { var e = await InventoryServiceHelper.RequireAsync(_db.StockTransfers.AsNoTracking().Where(x => x.StockTransferID == id), "Stock transfer", ct); var items = await _db.StockTransferItems.AsNoTracking().Where(x => x.StockTransferID == id).Select(x => new StockTransferItemDto(x.StockTransferItemID, x.SKUID, x.SKU.SKUCode, x.BatchID, x.Batch != null ? x.Batch.BatchNo : null, x.RequestedQuantity, x.DispatchedQuantity, x.ReceivedQuantity)).ToListAsync(ct); return new(e.StockTransferID, e.StockTransferNo, e.FromWarehouseID, e.ToWarehouseID, e.RequestedAt, e.DispatchedAt, e.ReceivedAt, e.Status, e.ApprovalRequestID, items); }

    public async Task<int> CreateAsync(SaveStockTransferRequestDto r, CancellationToken ct = default) { await ValidateAsync(r, null, ct); var e = new StockTransfer(); Apply(e, r); await _db.AddAsync(e, ct); foreach (var i in r.Items) await _db.AddAsync(new StockTransferItem { StockTransfer = e, SKUID = i.SKUID, BatchID = i.BatchID, RequestedQuantity = i.RequestedQuantity }, ct); await _db.SaveChangesAsync(ct); return e.StockTransferID; }
    public async Task UpdateAsync(int id, SaveStockTransferRequestDto r, CancellationToken ct = default) { var e = await InventoryServiceHelper.RequireAsync(_db.StockTransfers.Where(x => x.StockTransferID == id), "Stock transfer", ct); if (e.Status != StockTransferStatus.Draft) throw new BusinessRuleException("Only a draft stock transfer can be edited."); await ValidateAsync(r, id, ct); Apply(e, r); var old = await _db.StockTransferItems.Where(x => x.StockTransferID == id).ToListAsync(ct); foreach (var x in old) _db.Remove(x); foreach (var i in r.Items) await _db.AddAsync(new StockTransferItem { StockTransferID = id, SKUID = i.SKUID, BatchID = i.BatchID, RequestedQuantity = i.RequestedQuantity }, ct); await _db.SaveChangesAsync(ct); }
    public async Task SubmitAsync(int id, CancellationToken ct = default) { var e = await Require(id, ct); if (e.Status != StockTransferStatus.Draft) throw new BusinessRuleException("Only a draft transfer can be submitted."); e.Status = StockTransferStatus.Submitted; await _db.SaveChangesAsync(ct); }
    public async Task ApproveAsync(int id, CancellationToken ct = default) { var e = await Require(id, ct); if (e.Status != StockTransferStatus.Submitted) throw new BusinessRuleException("Only a submitted transfer can be approved."); e.Status = StockTransferStatus.Approved; await _db.SaveChangesAsync(ct); }

    public async Task DispatchAsync(int id, DispatchStockTransferRequestDto r, CancellationToken ct = default)
    {
        var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
        await _db.ExecuteInTransactionAsync(async token => { var e = await _db.StockTransfers.Include(x => x.Items).SingleOrDefaultAsync(x => x.StockTransferID == id, token) ?? throw new NotFoundException("Stock transfer was not found."); if (e.Status != StockTransferStatus.Approved) throw new BusinessRuleException("Only an approved transfer can be dispatched."); var values = r.Items.ToDictionary(x => x.StockTransferItemID, x => x.DispatchedQuantity); foreach (var i in e.Items) { if (!values.TryGetValue(i.StockTransferItemID, out var qty) || qty <= 0 || qty > i.RequestedQuantity) throw new BusinessRuleException("Dispatched quantity is invalid."); await InventoryServiceHelper.PostMovementAsync(_db, e.FromWarehouseID, i.SKUID, i.BatchID, StockMovementType.TransferOut, 0, qty, ReferenceTypeCodes.StockTransfer, e.StockTransferID, userID, r.Note, token); i.DispatchedQuantity = qty; } e.DispatchedAt = DateTime.UtcNow; e.Status = StockTransferStatus.Dispatched; await _db.SaveChangesAsync(token); return true; }, ct);
    }

    public async Task ReceiveAsync(int id, ReceiveStockTransferRequestDto r, CancellationToken ct = default)
    {
        var userID = _currentUser.UserID ?? throw new ForbiddenBusinessActionException("Authenticated user is required.");
        await _db.ExecuteInTransactionAsync(async token => { var e = await _db.StockTransfers.Include(x => x.Items).SingleOrDefaultAsync(x => x.StockTransferID == id, token) ?? throw new NotFoundException("Stock transfer was not found."); if (e.Status != StockTransferStatus.Dispatched && e.Status != StockTransferStatus.PartiallyReceived) throw new BusinessRuleException("Only a dispatched transfer can be received."); var values = r.Items.ToDictionary(x => x.StockTransferItemID, x => x.ReceivedQuantity); foreach (var i in e.Items) { if (!values.TryGetValue(i.StockTransferItemID, out var qty) || qty < i.ReceivedQuantity || qty > i.DispatchedQuantity) throw new BusinessRuleException("Received quantity is invalid."); var delta = qty - i.ReceivedQuantity; if (delta > 0) await InventoryServiceHelper.PostMovementAsync(_db, e.ToWarehouseID, i.SKUID, i.BatchID, StockMovementType.TransferIn, delta, 0, ReferenceTypeCodes.StockTransfer, e.StockTransferID, userID, r.Note, token); i.ReceivedQuantity = qty; } var complete = e.Items.All(x => x.ReceivedQuantity >= x.DispatchedQuantity); e.Status = complete ? StockTransferStatus.Received : StockTransferStatus.PartiallyReceived; if (complete) e.ReceivedAt = DateTime.UtcNow; await _db.SaveChangesAsync(token); return true; }, ct);
    }

    private async Task<StockTransfer> Require(int id, CancellationToken ct) => await InventoryServiceHelper.RequireAsync(_db.StockTransfers.Where(x => x.StockTransferID == id), "Stock transfer", ct);
    private async Task ValidateAsync(SaveStockTransferRequestDto r, int? id, CancellationToken ct) { if (string.IsNullOrWhiteSpace(r.StockTransferNo)) throw new BusinessRuleException("Stock transfer number is required."); if (r.FromWarehouseID == r.ToWarehouseID) throw new BusinessRuleException("Source and destination warehouses must be different."); InventoryServiceHelper.EnsureDistinctPositive(r.Items, x => HashCode.Combine(x.SKUID, x.BatchID), x => x.RequestedQuantity); var no = r.StockTransferNo.Trim().ToUpperInvariant(); if (await _db.StockTransfers.AnyAsync(x => x.StockTransferNo == no && x.StockTransferID != id, ct)) throw new ConflictException("Stock transfer number already exists."); var warehouses = await _db.Warehouses.CountAsync(x => (x.WarehouseID == r.FromWarehouseID || x.WarehouseID == r.ToWarehouseID) && x.IsActive, ct); if (warehouses != 2) throw new BusinessRuleException("Both warehouses must be active."); foreach (var i in r.Items) { if (i.BatchID.HasValue && !await _db.Batches.AnyAsync(x => x.BatchID == i.BatchID && x.SKUID == i.SKUID, ct)) throw new BusinessRuleException("Batch does not belong to the selected SKU."); } }
    private static void Apply(StockTransfer e, SaveStockTransferRequestDto r) { e.StockTransferNo = r.StockTransferNo.Trim().ToUpperInvariant(); e.FromWarehouseID = r.FromWarehouseID; e.ToWarehouseID = r.ToWarehouseID; e.RequestedAt = r.RequestedAt; }
}

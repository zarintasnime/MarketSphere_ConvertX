using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Modules.Inventory.DTOs;
using MarketSphere.Application.Modules.Inventory.Interfaces;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Inventory.Services;

public sealed class StockService : IStockService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;

    public StockService(IApplicationDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyCollection<StockBalanceDto>> GetBalancesAsync(
        StockSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StockBalances.AsNoTracking();

        if (request.WarehouseID.HasValue)
        {
            query = query.Where(x => x.WarehouseID == request.WarehouseID);
        }

        if (request.SKUID.HasValue)
        {
            query = query.Where(x => x.SKUID == request.SKUID);
        }

        if (request.BatchID.HasValue)
        {
            query = query.Where(x => x.BatchID == request.BatchID);
        }

        if (!request.IncludeZero)
        {
            query = query.Where(x =>
                x.OnHandQuantity != 0 ||
                x.ReservedQuantity != 0 ||
                x.QuarantineQuantity != 0 ||
                x.DamagedQuantity != 0);
        }

        var today = _clock.UtcNow.Date;
        if (!request.IncludeExpired)
        {
            query = query.Where(x =>
                x.BatchID == null ||
                x.Batch!.ExpiryDate == null ||
                x.Batch.ExpiryDate >= today);
        }

        return await query
            .OrderBy(x => x.Warehouse.WarehouseName)
            .ThenBy(x => x.SKU.SKUName)
            .ThenBy(x => x.Batch!.ExpiryDate)
            .Select(x => new StockBalanceDto(
                x.StockBalanceID,
                x.WarehouseID,
                x.Warehouse.WarehouseName,
                x.SKUID,
                x.SKU.SKUCode,
                x.SKU.SKUName,
                x.BatchID,
                x.Batch != null ? x.Batch.BatchNo : null,
                x.Batch != null ? x.Batch.ExpiryDate : null,
                x.OnHandQuantity,
                x.ReservedQuantity,
                x.QuarantineQuantity,
                x.DamagedQuantity,
                x.OnHandQuantity - x.ReservedQuantity - x.QuarantineQuantity - x.DamagedQuantity,
                x.RowVersion))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<BatchDto>> GetBatchesAsync(
        int? skuID,
        bool includeExpired,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Batches.AsNoTracking();

        if (skuID.HasValue)
        {
            query = query.Where(x => x.SKUID == skuID);
        }

        var today = _clock.UtcNow.Date;
        if (!includeExpired)
        {
            query = query.Where(x =>
                x.Status == BatchStatus.Available &&
                (x.ExpiryDate == null || x.ExpiryDate >= today));
        }

        return await query
            .OrderBy(x => x.ExpiryDate)
            .ThenBy(x => x.BatchNo)
            .Select(x => new BatchDto(
                x.BatchID,
                x.SKUID,
                x.SKU.SKUCode,
                x.BatchNo,
                x.ManufacturingDate,
                x.ExpiryDate,
                x.BestBeforeDate,
                x.CostPrice,
                x.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<StockMovementDto>> GetMovementsAsync(
        int? warehouseID,
        int? skuID,
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StockMovements.AsNoTracking();

        if (warehouseID.HasValue)
        {
            query = query.Where(x => x.WarehouseID == warehouseID);
        }

        if (skuID.HasValue)
        {
            query = query.Where(x => x.SKUID == skuID);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.MovementAt >= from);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.MovementAt < to.Value.AddDays(1));
        }

        return await query
            .OrderByDescending(x => x.StockMovementID)
            .Select(x => new StockMovementDto(
                x.StockMovementID,
                x.WarehouseID,
                x.Warehouse.WarehouseName,
                x.SKUID,
                x.SKU.SKUCode,
                x.SKU.SKUName,
                x.BatchID,
                x.Batch != null ? x.Batch.BatchNo : null,
                x.MovementType,
                x.QuantityIn,
                x.QuantityOut,
                x.BalanceAfter,
                x.ReferenceType,
                x.ReferenceID,
                x.MovementAt,
                x.Note))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<StockReservationDto>> GetReservationsAsync(
        int? orderItemID,
        CancellationToken cancellationToken = default)
    {
        var query = _db.StockReservations.AsNoTracking();

        if (orderItemID.HasValue)
        {
            query = query.Where(x => x.OrderItemID == orderItemID);
        }

        return await query
            .OrderByDescending(x => x.StockReservationID)
            .Select(x => new StockReservationDto(
                x.StockReservationID,
                x.OrderItemID,
                x.WarehouseID,
                x.Warehouse.WarehouseName,
                x.SKUID,
                x.SKU.SKUCode,
                x.SKU.SKUName,
                x.BatchID,
                x.Batch != null ? x.Batch.BatchNo : null,
                x.ReservedQuantity,
                x.ReservationStatus,
                x.ReservedAt,
                x.ExpiresAt,
                x.ReleasedAt))
            .ToListAsync(cancellationToken);
    }
}

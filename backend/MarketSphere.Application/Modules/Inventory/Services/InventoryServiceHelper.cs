using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.Inventory.Services;

internal static class InventoryServiceHelper
{
    public static async Task<T> RequireAsync<T>(IQueryable<T> query, string name, CancellationToken cancellationToken)
        where T : class
        => await query.SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException($"{name} was not found.");

    public static async Task<PagedResult<T>> ToPagedAsync<T>(IQueryable<T> query, PagedRequest request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.PageNumber);
        var size = Math.Clamp(request.PageSize, 1, 200);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return PagedResult<T>.Create(items, count, page, size);
    }

    public static void EnsureDistinctPositive<T>(IReadOnlyCollection<T> items, Func<T, int> key, Func<T, decimal> quantity)
    {
        if (items.Count == 0) throw new BusinessRuleException("At least one item is required.");
        if (items.Any(x => quantity(x) <= 0)) throw new BusinessRuleException("Quantity must be greater than zero.");
        if (items.GroupBy(key).Any(x => x.Count() > 1)) throw new BusinessRuleException("Duplicate items are not allowed.");
    }

    public static async Task<int?> GetOrCreateBatchAsync(
        IApplicationDbContext db,
        int skuID,
        string? batchNo,
        DateTime? manufacturingDate,
        DateTime? expiryDate,
        decimal costPrice,
        CancellationToken cancellationToken)
    {
        var sku = await db.SKUs.Include(x => x.Product)
            .SingleOrDefaultAsync(x => x.SKUID == skuID && x.IsActive, cancellationToken)
            ?? throw new BusinessRuleException("An active SKU is required.");

        if (!sku.Product.RequiresBatch) return null;
        if (string.IsNullOrWhiteSpace(batchNo)) throw new BusinessRuleException("Batch is required for this product.");
        if (sku.Product.RequiresExpiryDate && expiryDate is null)
            throw new BusinessRuleException("Expiry date is required for this product.");
        if (expiryDate.HasValue && manufacturingDate.HasValue && expiryDate.Value.Date < manufacturingDate.Value.Date)
            throw new BusinessRuleException("Expiry date cannot be earlier than manufacturing date.");

        var normalized = batchNo.Trim().ToUpperInvariant();
        var batch = await db.Batches.SingleOrDefaultAsync(
            x => x.SKUID == skuID && x.BatchNo == normalized,
            cancellationToken);
        if (batch is not null)
        {
            if (expiryDate.HasValue && batch.ExpiryDate.HasValue && batch.ExpiryDate.Value.Date != expiryDate.Value.Date)
                throw new BusinessRuleException("The existing batch has a different expiry date.");
            return batch.BatchID;
        }

        batch = new Batch
        {
            SKUID = skuID,
            BatchNo = normalized,
            ManufacturingDate = manufacturingDate?.Date,
            ExpiryDate = expiryDate?.Date,
            CostPrice = costPrice,
            Status = BatchStatus.Available
        };
        await db.AddAsync(batch, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return batch.BatchID;
    }

    public static async Task<StockMovement> PostMovementAsync(
        IApplicationDbContext db,
        int warehouseID,
        int skuID,
        int? batchID,
        StockMovementType movementType,
        decimal quantityIn,
        decimal quantityOut,
        string referenceType,
        int referenceID,
        int? performedByUserID,
        string? note,
        CancellationToken cancellationToken)
    {
        if ((quantityIn > 0) == (quantityOut > 0))
            throw new BusinessRuleException("Exactly one movement quantity must be greater than zero.");

        var warehouseActive = await db.Warehouses.AnyAsync(
            x => x.WarehouseID == warehouseID && x.IsActive,
            cancellationToken);
        if (!warehouseActive) throw new BusinessRuleException("An active warehouse is required.");

        var sku = await db.SKUs.Include(x => x.Product)
            .SingleOrDefaultAsync(x => x.SKUID == skuID && x.IsActive, cancellationToken)
            ?? throw new BusinessRuleException("An active SKU is required.");

        if (sku.Product.RequiresBatch && batchID is null)
            throw new BusinessRuleException("Batch is required for this product.");
        if (!sku.Product.RequiresBatch) batchID = null;

        if (batchID.HasValue)
        {
            var batch = await db.Batches.SingleOrDefaultAsync(
                x => x.BatchID == batchID.Value && x.SKUID == skuID,
                cancellationToken)
                ?? throw new BusinessRuleException("The batch does not belong to the selected SKU.");
            if (quantityOut > 0 && (batch.Status != BatchStatus.Available || batch.ExpiryDate < DateTime.UtcNow.Date))
                throw new BusinessRuleException("Expired, blocked or unavailable batch stock cannot be issued.");
        }

        var balance = await db.StockBalances.SingleOrDefaultAsync(
            x => x.WarehouseID == warehouseID && x.SKUID == skuID && x.BatchID == batchID,
            cancellationToken);
        if (balance is null)
        {
            balance = new StockBalance { WarehouseID = warehouseID, SKUID = skuID, BatchID = batchID };
            await db.AddAsync(balance, cancellationToken);
        }

        var newOnHand = balance.OnHandQuantity + quantityIn - quantityOut;
        if (newOnHand < 0) throw new BusinessRuleException("The operation would create a negative stock balance.");
        if (newOnHand < balance.ReservedQuantity + balance.QuarantineQuantity + balance.DamagedQuantity)
            throw new BusinessRuleException("The operation exceeds available stock.");

        balance.OnHandQuantity = newOnHand;
        var movement = new StockMovement
        {
            WarehouseID = warehouseID,
            SKUID = skuID,
            BatchID = batchID,
            MovementType = movementType,
            QuantityIn = quantityIn,
            QuantityOut = quantityOut,
            BalanceAfter = newOnHand,
            ReferenceType = referenceType,
            ReferenceID = referenceID,
            MovementAt = DateTime.UtcNow,
            PerformedByUserID = performedByUserID,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
        };
        await db.AddAsync(movement, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return movement;
    }
}

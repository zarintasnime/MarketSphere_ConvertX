using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.Inventory;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

internal static class OrderFulfilmentServiceHelper
{
    public static async Task<T> RequireAsync<T>(
        IQueryable<T> query,
        string name,
        CancellationToken cancellationToken)
        where T : class
        => await query.SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException($"{name} was not found.");

    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        IQueryable<T> query,
        PagedRequest request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.PageNumber);
        var size = Math.Clamp(request.PageSize, 1, 200);
        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(items, count, page, size);
    }

    public static void ValidateOrderItems<T>(
        IReadOnlyCollection<T> items,
        Func<T, int> sku,
        Func<T, decimal> quantity)
    {
        if (items.Count == 0)
            throw new BusinessRuleException("At least one order item is required.");

        if (items.Any(x => quantity(x) <= 0))
            throw new BusinessRuleException(BusinessRuleMessages.QuantityMustBePositive);

        if (items.GroupBy(sku).Any(x => x.Count() > 1))
            throw new BusinessRuleException("Duplicate SKU lines are not allowed.");
    }

    public static async Task<CreditCheckStatus> EvaluateCreditAsync(
        IApplicationDbContext db,
        int clientID,
        decimal orderAmount,
        CancellationToken cancellationToken)
    {
        var profile = await db.ClientCreditProfiles
            .SingleOrDefaultAsync(x => x.ClientID == clientID, cancellationToken);

        if (profile is null)
            return CreditCheckStatus.NotRequired;

        if (profile.IsBlocked)
            return CreditCheckStatus.Failed;

        return profile.CurrentDue + orderAmount <= profile.CreditLimit
            ? CreditCheckStatus.Passed
            : CreditCheckStatus.OverrideRequired;
    }

    public static async Task ReserveOrderItemFefoAsync(
        IApplicationDbContext db,
        OrderItem item,
        int warehouseID,
        DateTime? expiresAt,
        CancellationToken cancellationToken)
    {
        var alreadyReserved = await db.StockReservations.AnyAsync(
            x => x.OrderItemID == item.OrderItemID &&
                 x.ReservationStatus == StockReservationStatus.Active,
            cancellationToken);

        if (alreadyReserved)
            throw new BusinessRuleException(BusinessRuleMessages.OrderAlreadyReserved);

        var sku = await db.SKUs
            .Include(x => x.Product)
            .SingleAsync(x => x.SKUID == item.SKUID, cancellationToken);

        var required = item.ApprovedQuantity > 0
            ? item.ApprovedQuantity
            : item.OrderedQuantity + item.FreeQuantity;

        var balances = await db.StockBalances
            .Include(x => x.Batch)
            .Where(x => x.WarehouseID == warehouseID && x.SKUID == item.SKUID)
            .OrderBy(x => x.BatchID == null ? 1 : 0)
            .ThenBy(x => x.Batch!.ExpiryDate)
            .ThenBy(x => x.BatchID)
            .ToListAsync(cancellationToken);

        var remaining = required;

        foreach (var balance in balances)
        {
            if (remaining <= 0)
                break;

            if (sku.Product.RequiresBatch && balance.BatchID is null)
                continue;

            if (balance.Batch is not null &&
                (balance.Batch.Status != BatchStatus.Available ||
                 (balance.Batch.ExpiryDate.HasValue &&
                  balance.Batch.ExpiryDate.Value.Date < DateTime.UtcNow.Date)))
            {
                continue;
            }

            var available = balance.OnHandQuantity -
                            balance.ReservedQuantity -
                            balance.QuarantineQuantity -
                            balance.DamagedQuantity;

            if (available <= 0)
                continue;

            var quantity = Math.Min(remaining, available);
            balance.ReservedQuantity += quantity;

            await db.AddAsync(
                new StockReservation
                {
                    OrderItemID = item.OrderItemID,
                    WarehouseID = warehouseID,
                    SKUID = item.SKUID,
                    BatchID = balance.BatchID,
                    ReservedQuantity = quantity,
                    ReservationStatus = StockReservationStatus.Active,
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt
                },
                cancellationToken);

            remaining -= quantity;
        }

        if (remaining > 0)
            throw new BusinessRuleException(BusinessRuleMessages.InsufficientFefoStock);

        await db.SaveChangesAsync(cancellationToken);
    }

    public static async Task ConsumeReservationAsync(
        IApplicationDbContext db,
        StockReservation reservation,
        decimal quantityToConsume,
        int performedByUserID,
        int deliveryID,
        CancellationToken cancellationToken)
    {
        if (reservation.ReservationStatus != StockReservationStatus.Active)
            throw new BusinessRuleException("Only an active reservation can be consumed.");

        if (quantityToConsume <= 0 || quantityToConsume > reservation.ReservedQuantity)
            throw new BusinessRuleException("The dispatched quantity exceeds the active reservation.");

        var balance = await RequireAsync(
            db.StockBalances.Where(x =>
                x.WarehouseID == reservation.WarehouseID &&
                x.SKUID == reservation.SKUID &&
                x.BatchID == reservation.BatchID),
            "Stock balance",
            cancellationToken);

        if (balance.ReservedQuantity < reservation.ReservedQuantity)
            throw new BusinessRuleException("Reserved stock balance is inconsistent.");
        balance.ReservedQuantity -= reservation.ReservedQuantity;
        reservation.ReservationStatus = StockReservationStatus.Consumed;
        reservation.ReleasedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        await InventoryServiceHelper.PostMovementAsync(
            db,
            reservation.WarehouseID,
            reservation.SKUID,
            reservation.BatchID,
            StockMovementType.DeliveryIssue,
            0,
            quantityToConsume,
            ReferenceTypeCodes.Delivery,
            deliveryID,
            performedByUserID,
            "Delivery dispatch",
            cancellationToken);
    }

    public static async Task<StockReservation> RestoreAndReserveDeliveryItemAsync(
        IApplicationDbContext db,
        Delivery delivery,
        DeliveryItem deliveryItem,
        int performedByUserID,
        CancellationToken cancellationToken)
    {
        if (deliveryItem.QuantityDispatched <= 0)
            throw new BusinessRuleException("A positive dispatched quantity is required.");

        await InventoryServiceHelper.PostMovementAsync(
            db,
            delivery.WarehouseID,
            deliveryItem.SKUID,
            deliveryItem.BatchID,
            StockMovementType.CustomerReturn,
            deliveryItem.QuantityDispatched,
            0,
            ReferenceTypeCodes.Delivery,
            delivery.DeliveryID,
            performedByUserID,
            "Unsuccessful delivery returned to warehouse",
            cancellationToken);

        var balance = await RequireAsync(
            db.StockBalances.Where(x =>
                x.WarehouseID == delivery.WarehouseID &&
                x.SKUID == deliveryItem.SKUID &&
                x.BatchID == deliveryItem.BatchID),
            "Stock balance",
            cancellationToken);

        balance.ReservedQuantity += deliveryItem.QuantityDispatched;

        var reservation = new StockReservation
        {
            OrderItemID = deliveryItem.OrderItemID,
            WarehouseID = delivery.WarehouseID,
            SKUID = deliveryItem.SKUID,
            BatchID = deliveryItem.BatchID,
            ReservedQuantity = deliveryItem.QuantityDispatched,
            ReservationStatus = StockReservationStatus.Active,
            ReservedAt = DateTime.UtcNow,
            ExpiresAt = delivery.RescheduledDate
        };

        await db.AddAsync(reservation, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        if (deliveryItem.PickListItemID.HasValue)
        {
            var pickItem = await db.PickListItems.SingleAsync(
                x => x.PickListItemID == deliveryItem.PickListItemID.Value,
                cancellationToken);

            pickItem.StockReservationID = reservation.StockReservationID;
            await db.SaveChangesAsync(cancellationToken);
        }

        return reservation;
    }

    public static async Task UpdateClientDueAsync(
        IApplicationDbContext db,
        int clientID,
        decimal delta,
        CancellationToken cancellationToken)
    {
        var profile = await db.ClientCreditProfiles
            .SingleOrDefaultAsync(x => x.ClientID == clientID, cancellationToken);

        if (profile is null)
            return;

        var next = profile.CurrentDue + delta;

        if (next < 0)
            throw new BusinessRuleException(BusinessRuleMessages.CurrentDueCannotBeNegative);

        profile.CurrentDue = next;
        await db.SaveChangesAsync(cancellationToken);
    }
}

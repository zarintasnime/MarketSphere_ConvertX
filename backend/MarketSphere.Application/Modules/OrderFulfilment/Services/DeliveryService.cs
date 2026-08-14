using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.Inventory.Services;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

public sealed class DeliveryService : IDeliveryService
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ICurrentUserService _currentUser;

    public DeliveryService(
        IApplicationDbContext db,
        IDateTimeProvider clock,
        ICurrentUserService currentUser)
    {
        _db = db;
        _clock = clock;
        _currentUser = currentUser;
    }

    public Task<PagedResult<DeliveryListDto>> GetAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Deliveries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.DeliveryNo.Contains(search) ||
                x.Order.OrderNo.Contains(search));
        }

        var projected = query
            .OrderByDescending(x => x.DeliveryID)
            .Select(x => new DeliveryListDto(
                x.DeliveryID,
                x.DeliveryNo,
                x.OrderID,
                x.InvoiceID,
                x.PickListID,
                x.WarehouseID,
                x.PlannedDeliveryDate,
                x.DispatchDate,
                x.DeliveredAt,
                x.Status));

        return OrderFulfilmentServiceHelper.ToPagedAsync(
            projected,
            request,
            cancellationToken);
    }

    public async Task<DeliveryDetailsDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var delivery = await OrderFulfilmentServiceHelper.RequireAsync(
            _db.Deliveries.AsNoTracking().Where(x => x.DeliveryID == id),
            "Delivery",
            cancellationToken);

        var items = await _db.DeliveryItems
            .AsNoTracking()
            .Where(x => x.DeliveryID == id)
            .OrderBy(x => x.DeliveryItemID)
            .Select(x => new DeliveryItemDto(
                x.DeliveryItemID,
                x.PickListItemID,
                x.OrderItemID,
                x.InvoiceItemID,
                x.SKUID,
                x.SKU.SKUCode,
                x.BatchID,
                x.QuantityDispatched,
                x.QuantityDelivered,
                x.QuantityRejectedAtDelivery))
            .ToListAsync(cancellationToken);

        return new DeliveryDetailsDto(
            delivery.DeliveryID,
            delivery.DeliveryNo,
            delivery.OrderID,
            delivery.InvoiceID,
            delivery.PickListID,
            delivery.WarehouseID,
            delivery.PlannedDeliveryDate,
            delivery.DispatchDate,
            delivery.DeliveredAt,
            delivery.Status,
            delivery.DeliveredByEmployeeID,
            delivery.ReceiverName,
            delivery.ReceiverPhone,
            delivery.FailureReason,
            delivery.RescheduledDate,
            items);
    }

    public async Task<int> CreateAsync(
        CreateDeliveryRequestDto request,
        CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            if (string.IsNullOrWhiteSpace(request.DeliveryNo))
                throw new BusinessRuleException("Delivery number is required.");

            var number = request.DeliveryNo.Trim().ToUpperInvariant();

            if (await _db.Deliveries.AnyAsync(x => x.DeliveryNo == number, ct))
                throw new ConflictException("Delivery number already exists.");

            if (await _db.Deliveries.AnyAsync(
                    x => x.PickListID == request.PickListID &&
                         x.Status != DeliveryStatus.Cancelled,
                    ct))
            {
                throw new ConflictException(
                    "A delivery already exists for the selected pick list.");
            }

            var pickList = await OrderFulfilmentServiceHelper.RequireAsync(
                _db.PickLists.Where(x => x.PickListID == request.PickListID),
                "Pick list",
                ct);

            if (pickList.Status != PickListStatus.Verified)
                throw new BusinessRuleException(
                    BusinessRuleMessages.PickListVerificationRequired);

            if (pickList.OrderID != request.OrderID ||
                pickList.WarehouseID != request.WarehouseID)
            {
                throw new BusinessRuleException(
                    "Delivery, order, pick list and warehouse are inconsistent.");
            }

            if (request.InvoiceID.HasValue &&
                !await _db.Invoices.AnyAsync(
                    x => x.InvoiceID == request.InvoiceID.Value &&
                         x.OrderID == request.OrderID &&
                         x.Status != InvoiceStatus.Cancelled,
                    ct))
            {
                throw new BusinessRuleException(
                    "The active invoice does not belong to the selected order.");
            }

            var pickItems = await _db.PickListItems
                .Where(x => x.PickListID == pickList.PickListID &&
                            x.PickedQuantity > 0)
                .ToListAsync(ct);

            if (pickItems.Count == 0)
                throw new BusinessRuleException(
                    "The verified pick list has no picked quantity to deliver.");

            var delivery = new Delivery
            {
                DeliveryNo = number,
                OrderID = request.OrderID,
                InvoiceID = request.InvoiceID,
                PickListID = request.PickListID,
                WarehouseID = request.WarehouseID,
                PlannedDeliveryDate = request.PlannedDeliveryDate,
                Status = DeliveryStatus.ReadyForDispatch
            };

            await _db.AddAsync(delivery, ct);

            foreach (var pickItem in pickItems)
            {
                var invoiceItemID = request.InvoiceID.HasValue
                    ? await _db.InvoiceItems
                        .Where(x =>
                            x.InvoiceID == request.InvoiceID.Value &&
                            x.OrderItemID == pickItem.OrderItemID)
                        .Select(x => (int?)x.InvoiceItemID)
                        .FirstOrDefaultAsync(ct)
                    : null;

                await _db.AddAsync(
                    new DeliveryItem
                    {
                        Delivery = delivery,
                        PickListItemID = pickItem.PickListItemID,
                        OrderItemID = pickItem.OrderItemID,
                        InvoiceItemID = invoiceItemID,
                        SKUID = pickItem.SKUID,
                        BatchID = pickItem.BatchID,
                        QuantityDispatched = pickItem.PickedQuantity
                    },
                    ct);
            }

            await _db.SaveChangesAsync(ct);
            return delivery.DeliveryID;
        }, cancellationToken);

    public async Task DispatchAsync(
        int id,
        DispatchDeliveryRequestDto request,
        CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var userID = _currentUser.UserID
                ?? throw new ForbiddenBusinessActionException(
                    "Authenticated user is required.");

            var delivery = await _db.Deliveries
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.DeliveryID == id, ct)
                ?? throw new NotFoundException("Delivery was not found.");

            if (delivery.Status is not (
                DeliveryStatus.ReadyForDispatch or
                DeliveryStatus.Failed or
                DeliveryStatus.Rescheduled))
            {
                throw new BusinessRuleException(
                    "Only a ready, failed or rescheduled delivery can be dispatched.");
            }

            var pickList = await OrderFulfilmentServiceHelper.RequireAsync(
                _db.PickLists.Where(x => x.PickListID == delivery.PickListID),
                "Pick list",
                ct);

            if (pickList.Status != PickListStatus.Verified)
                throw new BusinessRuleException(
                    BusinessRuleMessages.PickListVerificationRequired);

            if (!await _db.Employees.AnyAsync(
                    x => x.EmployeeID == request.DeliveredByEmployeeID,
                    ct))
            {
                throw new NotFoundException("Delivery employee was not found.");
            }

            if (delivery.Items.Count == 0)
                throw new BusinessRuleException("The delivery has no items.");

            foreach (var deliveryItem in delivery.Items)
            {
                if (!deliveryItem.PickListItemID.HasValue)
                    throw new BusinessRuleException(
                        "Every delivery item must reference a pick-list item.");

                var pickItem = await _db.PickListItems
                    .SingleAsync(
                        x => x.PickListItemID == deliveryItem.PickListItemID.Value,
                        ct);

                if (!pickItem.StockReservationID.HasValue)
                    throw new BusinessRuleException(
                        "The pick-list item has no active stock reservation.");

                var reservation = await OrderFulfilmentServiceHelper.RequireAsync(
                    _db.StockReservations.Where(x =>
                        x.StockReservationID == pickItem.StockReservationID.Value &&
                        x.OrderItemID == deliveryItem.OrderItemID &&
                        x.WarehouseID == delivery.WarehouseID &&
                        x.SKUID == deliveryItem.SKUID &&
                        x.BatchID == deliveryItem.BatchID &&
                        x.ReservationStatus == StockReservationStatus.Active),
                    "Active stock reservation",
                    ct);

                await OrderFulfilmentServiceHelper.ConsumeReservationAsync(
                    _db,
                    reservation,
                    deliveryItem.QuantityDispatched,
                    userID,
                    delivery.DeliveryID,
                    ct);
            }

            delivery.Status = DeliveryStatus.Dispatched;
            delivery.DispatchDate = _clock.UtcNow;
            delivery.DeliveredByEmployeeID = request.DeliveredByEmployeeID;
            delivery.DeliveredAt = null;
            delivery.ReceiverName = null;
            delivery.ReceiverPhone = null;
            delivery.FailureReason = null;
            delivery.RescheduledDate = null;

            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);

    public async Task CompleteAsync(
        int id,
        CompleteDeliveryRequestDto request,
        CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var userID = _currentUser.UserID
                ?? throw new ForbiddenBusinessActionException(
                    "Authenticated user is required.");

            var delivery = await _db.Deliveries
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.DeliveryID == id, ct)
                ?? throw new NotFoundException("Delivery was not found.");

            if (delivery.Status != DeliveryStatus.Dispatched)
                throw new BusinessRuleException(
                    "Only a dispatched delivery can be completed.");

            if (request.Status is DeliveryStatus.Failed or DeliveryStatus.Rescheduled)
            {
                if (string.IsNullOrWhiteSpace(request.FailureReason))
                    throw new BusinessRuleException("Failure reason is required.");

                if (request.Status == DeliveryStatus.Rescheduled &&
                    !request.RescheduledDate.HasValue)
                {
                    throw new BusinessRuleException(
                        "A rescheduled delivery requires a new delivery date.");
                }

                delivery.Status = request.Status;
                delivery.FailureReason = request.FailureReason.Trim();
                delivery.RescheduledDate = request.RescheduledDate;

                foreach (var deliveryItem in delivery.Items)
                {
                    await OrderFulfilmentServiceHelper
                        .RestoreAndReserveDeliveryItemAsync(
                            _db,
                            delivery,
                            deliveryItem,
                            userID,
                            ct);
                }

                var failedOrder = await _db.Orders.SingleAsync(
                    x => x.OrderID == delivery.OrderID,
                    ct);
                failedOrder.Status = OrderStatus.ReadyForDispatch;

                await _db.SaveChangesAsync(ct);
                return true;
            }

            if (request.Status is not (
                DeliveryStatus.Delivered or
                DeliveryStatus.PartiallyDelivered))
            {
                throw new BusinessRuleException(
                    "Completion status must be Delivered or PartiallyDelivered.");
            }

            if (request.Items.Count != delivery.Items.Count ||
                request.Items
                    .GroupBy(x => x.DeliveryItemID)
                    .Any(x => x.Count() > 1))
            {
                throw new BusinessRuleException(
                    "Every delivery item must be supplied exactly once.");
            }

            var itemMap = request.Items.ToDictionary(x => x.DeliveryItemID);

            foreach (var deliveryItem in delivery.Items)
            {
                if (!itemMap.TryGetValue(deliveryItem.DeliveryItemID, out var input))
                    throw new BusinessRuleException(
                        "Every delivery item must be supplied exactly once.");

                if (input.QuantityDelivered < 0 ||
                    input.QuantityRejectedAtDelivery < 0 ||
                    input.QuantityDelivered + input.QuantityRejectedAtDelivery !=
                    deliveryItem.QuantityDispatched)
                {
                    throw new BusinessRuleException(
                        BusinessRuleMessages.DeliveryQuantityInvalid);
                }

                deliveryItem.QuantityDelivered = input.QuantityDelivered;
                deliveryItem.QuantityRejectedAtDelivery =
                    input.QuantityRejectedAtDelivery;

                var orderItem = await _db.OrderItems.SingleAsync(
                    x => x.OrderItemID == deliveryItem.OrderItemID,
                    ct);

                orderItem.DeliveredQuantity += input.QuantityDelivered;
                orderItem.BackorderQuantity = Math.Max(
                    0,
                    orderItem.ApprovedQuantity - orderItem.DeliveredQuantity);

                if (input.QuantityRejectedAtDelivery > 0)
                {
                    await InventoryServiceHelper.PostMovementAsync(
                        _db,
                        delivery.WarehouseID,
                        deliveryItem.SKUID,
                        deliveryItem.BatchID,
                        StockMovementType.CustomerReturn,
                        input.QuantityRejectedAtDelivery,
                        0,
                        ReferenceTypeCodes.Delivery,
                        delivery.DeliveryID,
                        userID,
                        "Rejected at delivery",
                        ct);
                }
            }

            var hasRejectedQuantity = delivery.Items.Any(
                x => x.QuantityRejectedAtDelivery > 0);

            if (request.Status == DeliveryStatus.Delivered && hasRejectedQuantity)
                throw new BusinessRuleException(
                    "A fully delivered status cannot contain rejected quantity.");

            if (request.Status == DeliveryStatus.PartiallyDelivered &&
                !hasRejectedQuantity)
            {
                throw new BusinessRuleException(
                    "A partially delivered status requires rejected quantity.");
            }

            delivery.Status = request.Status;
            delivery.DeliveredAt = _clock.UtcNow;
            delivery.ReceiverName = string.IsNullOrWhiteSpace(request.ReceiverName)
                ? null
                : request.ReceiverName.Trim();
            delivery.ReceiverPhone = string.IsNullOrWhiteSpace(request.ReceiverPhone)
                ? null
                : request.ReceiverPhone.Trim();
            delivery.FailureReason = null;
            delivery.RescheduledDate = null;

            var order = await _db.Orders.SingleAsync(
                x => x.OrderID == delivery.OrderID,
                ct);

            var orderItems = await _db.OrderItems
                .Where(x => x.OrderID == order.OrderID)
                .ToListAsync(ct);

            order.Status = orderItems.All(
                x => x.DeliveredQuantity >= x.ApprovedQuantity)
                ? OrderStatus.Delivered
                : OrderStatus.PartiallyDelivered;

            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);
}

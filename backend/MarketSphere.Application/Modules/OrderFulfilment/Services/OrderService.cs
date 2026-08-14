using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Constants;
using MarketSphere.Domain.Entities.OrderFulfilment;
using MarketSphere.Domain.Enums;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.OrderFulfilment.Services;

public sealed class OrderService : IOrderService
{
    private readonly IApplicationDbContext _db;
    public OrderService(IApplicationDbContext db) => _db = db;

    public Task<PagedResult<OrderListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _db.Orders.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search)) { var s = request.Search.Trim(); query = query.Where(x => x.OrderNo.Contains(s) || x.Client.ClientName.Contains(s)); }
        return OrderFulfilmentServiceHelper.ToPagedAsync(query.OrderByDescending(x => x.OrderDate).Select(x => new OrderListDto(x.OrderID, x.OrderNo, x.ClientID, x.Client.ClientName, x.Channel, x.OrderSource, x.OrderDate, x.Status, x.CreditCheckStatus, x.NetAmount)), request, cancellationToken);
    }

    public async Task<OrderDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await OrderFulfilmentServiceHelper.RequireAsync(_db.Orders.AsNoTracking().Where(x => x.OrderID == id), "Order", cancellationToken);
        var items = await _db.OrderItems.AsNoTracking().Where(x => x.OrderID == id).OrderBy(x => x.OrderItemID).Select(x => new OrderItemDto(x.OrderItemID, x.SKUID, x.SKU.SKUCode, x.SKU.SKUName, x.OrderedQuantity, x.FreeQuantity, x.UnitPrice, x.DiscountPercent, x.DiscountAmount, x.TaxAmount, x.LineTotal, x.ApprovedQuantity, x.DeliveredQuantity, x.ReturnedQuantity, x.BackorderQuantity)).ToListAsync(cancellationToken);
        return new(order.OrderID, order.OrderNo, order.ClientID, order.EmployeeID, order.Channel, order.OrderSource, order.CampaignID, order.QuotationID, order.ModernTradePurchaseOrderID, order.PriceListID, order.OrderDate, order.RequestedDeliveryDate, order.DeliveryAddressSnapshot, order.Status, order.GrossAmount, order.DiscountAmount, order.TaxAmount, order.NetAmount, order.CreditCheckStatus, order.ApprovalRequestID, items);
    }

    public async Task<int> CreateRegularAsync(SaveRegularOrderRequestDto request, CancellationToken cancellationToken = default)
    {
        await ValidateRegularAsync(request, cancellationToken);
        var order = new Order { OrderSource = request.CampaignID.HasValue ? OrderSource.Campaign : OrderSource.Regular, Status = OrderStatus.Draft };
        ApplyRegular(order, request);
        await _db.AddAsync(order, cancellationToken);
        foreach (var input in request.Items) await _db.AddAsync(ToOrderItem(order, input), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return order.OrderID;
    }

    public async Task<int> ConvertQuotationAsync(ConvertQuotationToOrderRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var quotation = await OrderFulfilmentServiceHelper.RequireAsync(_db.Quotations.Where(x => x.QuotationID == request.QuotationID), "Quotation", ct);
            if (quotation.Status != QuotationStatus.Accepted) throw new BusinessRuleException("Only an accepted quotation can be converted.");
            if (await _db.Orders.AnyAsync(x => x.QuotationID == request.QuotationID, ct)) throw new ConflictException(BusinessRuleMessages.SourceAlreadyConverted);
            await EnsureOrderNumberUniqueAsync(request.OrderNo, null, ct);
            var items = await _db.QuotationItems.AsNoTracking().Where(x => x.QuotationID == request.QuotationID).ToListAsync(ct);
            if (items.Count == 0) throw new BusinessRuleException("The quotation has no items.");
            var clientChannel = await _db.Clients.Where(x => x.ClientID == quotation.ClientID).Select(x => x.Channel).SingleAsync(ct);
            var order = new Order { OrderNo = request.OrderNo.Trim().ToUpperInvariant(), ClientID = quotation.ClientID, EmployeeID = request.EmployeeID, Channel = clientChannel, OrderSource = OrderSource.Quotation, CampaignID = quotation.CampaignID, QuotationID = quotation.QuotationID, PriceListID = quotation.PriceListID, OrderDate = request.OrderDate, RequestedDeliveryDate = request.RequestedDeliveryDate, DeliveryAddressSnapshot = request.DeliveryAddressSnapshot.Trim(), Status = OrderStatus.Draft, GrossAmount = quotation.GrossAmount, DiscountAmount = quotation.DiscountAmount, TaxAmount = quotation.TaxAmount, NetAmount = quotation.NetAmount };
            await _db.AddAsync(order, ct);
            foreach (var source in items) await _db.AddAsync(new OrderItem { Order = order, SKUID = source.SKUID, OrderedQuantity = source.Quantity, UnitPrice = source.UnitPrice, DiscountPercent = source.DiscountPercent, DiscountAmount = source.DiscountAmount, TaxAmount = source.TaxAmount, LineTotal = source.LineTotal, ApprovedQuantity = source.Quantity, BackorderQuantity = source.Quantity }, ct);
            quotation.Status = QuotationStatus.Converted;
            await _db.SaveChangesAsync(ct);
            return order.OrderID;
        }, cancellationToken);

    public async Task<int> ConvertModernTradePurchaseOrderAsync(ConvertModernTradePurchaseOrderRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var mtpo = await OrderFulfilmentServiceHelper.RequireAsync(_db.ModernTradePurchaseOrders.Where(x => x.ModernTradePurchaseOrderID == request.ModernTradePurchaseOrderID), "Modern-trade purchase order", ct);
            if (mtpo.Status != ModernTradePurchaseOrderStatus.Verified || mtpo.VerificationStatus != ModernTradeVerificationStatus.Verified || mtpo.CompletenessStatus != ModernTradeCompletenessStatus.Complete) throw new BusinessRuleException(BusinessRuleMessages.ModernTradePurchaseOrderNotVerified);
            if (await _db.Orders.AnyAsync(x => x.ModernTradePurchaseOrderID == request.ModernTradePurchaseOrderID, ct)) throw new ConflictException(BusinessRuleMessages.SourceAlreadyConverted);
            await EnsureOrderNumberUniqueAsync(request.OrderNo, null, ct);
            var items = await _db.ModernTradePurchaseOrderItems.AsNoTracking().Where(x => x.ModernTradePurchaseOrderID == request.ModernTradePurchaseOrderID).ToListAsync(ct);
            if (items.Any(x => !x.SKUID.HasValue || !x.AgreedUnitPrice.HasValue)) throw new BusinessRuleException("Every modern-trade line requires an internal SKU and agreed unit price.");
            var order = new Order { OrderNo = request.OrderNo.Trim().ToUpperInvariant(), ClientID = mtpo.ClientID, EmployeeID = request.EmployeeID, Channel = SalesChannel.ModernTrade, OrderSource = OrderSource.ModernTradePurchaseOrder, ModernTradePurchaseOrderID = mtpo.ModernTradePurchaseOrderID, PriceListID = request.PriceListID, OrderDate = request.OrderDate, RequestedDeliveryDate = mtpo.RequestedDeliveryDate, DeliveryAddressSnapshot = request.DeliveryAddressSnapshot.Trim(), Status = OrderStatus.Draft };
            await _db.AddAsync(order, ct);
            foreach (var source in items)
            {
                var gross = source.OrderedQuantity * source.AgreedUnitPrice!.Value; var discount = source.Discount ?? 0; var total = gross - discount;
                await _db.AddAsync(new OrderItem { Order = order, SKUID = source.SKUID!.Value, OrderedQuantity = source.OrderedQuantity, UnitPrice = source.AgreedUnitPrice.Value, DiscountAmount = discount, LineTotal = total, ApprovedQuantity = source.OrderedQuantity, BackorderQuantity = source.OrderedQuantity }, ct);
                order.GrossAmount += gross; order.DiscountAmount += discount; order.NetAmount += total;
            }
            mtpo.Status = ModernTradePurchaseOrderStatus.Converted;
            await _db.SaveChangesAsync(ct);
            return order.OrderID;
        }, cancellationToken);

    public async Task SubmitAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await OrderFulfilmentServiceHelper.RequireAsync(_db.Orders.Where(x => x.OrderID == id), "Order", cancellationToken);
        if (order.Status != OrderStatus.Draft) throw new BusinessRuleException("Only a draft order can be submitted.");
        if (!await _db.OrderItems.AnyAsync(x => x.OrderID == id, cancellationToken)) throw new BusinessRuleException("The order has no items.");
        order.CreditCheckStatus = await OrderFulfilmentServiceHelper.EvaluateCreditAsync(_db, order.ClientID, order.NetAmount, cancellationToken);
        order.Status = order.CreditCheckStatus is CreditCheckStatus.Failed or CreditCheckStatus.OverrideRequired ? OrderStatus.UnderReview : OrderStatus.Submitted;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAndReserveAsync(int id, ApproveAndReserveOrderRequestDto request, CancellationToken cancellationToken = default)
        => await _db.ExecuteInTransactionAsync(async ct =>
        {
            var order = await OrderFulfilmentServiceHelper.RequireAsync(_db.Orders.Where(x => x.OrderID == id), "Order", ct);
            if (order.Status is not (OrderStatus.Submitted or OrderStatus.UnderReview or OrderStatus.Approved)) throw new BusinessRuleException("The order is not ready for approval and reservation.");
            if (order.CreditCheckStatus is CreditCheckStatus.Failed or CreditCheckStatus.OverrideRequired)
            {
                if (!request.ApprovalRequestID.HasValue)
                    throw new BusinessRuleException(BusinessRuleMessages.OrderCreditCheckFailed);

                var approval = await _db.ApprovalRequests.SingleOrDefaultAsync(
                    x => x.ApprovalRequestID == request.ApprovalRequestID.Value &&
                         x.ReferenceType == ReferenceTypeCodes.Order &&
                         x.ReferenceID == order.OrderID &&
                         x.Status == ApprovalRequestStatus.Approved &&
                         (x.ApprovalType == ApprovalType.Order ||
                          x.ApprovalType == ApprovalType.CreditOverride),
                    ct);

                if (approval is null)
                {
                    throw new BusinessRuleException(
                        "An approved order or credit-override request linked to this order is required.");
                }

                order.ApprovalRequestID = approval.ApprovalRequestID;
            }
            if (!await _db.Warehouses.AnyAsync(x => x.WarehouseID == request.WarehouseID && x.IsActive, ct)) throw new NotFoundException("Active warehouse was not found.");
            var items = await _db.OrderItems.Where(x => x.OrderID == id).ToListAsync(ct);
            foreach (var item in items)
            {
                if (item.ApprovedQuantity <= 0) item.ApprovedQuantity = item.OrderedQuantity + item.FreeQuantity;
                item.BackorderQuantity = item.ApprovedQuantity;
                await OrderFulfilmentServiceHelper.ReserveOrderItemFefoAsync(_db, item, request.WarehouseID, request.ReservationExpiresAt, ct);
            }
            order.Status = OrderStatus.StockAllocated;
            await _db.SaveChangesAsync(ct);
            return true;
        }, cancellationToken);

    public async Task ChangeStatusAsync(int id, ChangeOrderStatusRequestDto request, CancellationToken cancellationToken = default)
    {
        var order = await OrderFulfilmentServiceHelper.RequireAsync(_db.Orders.Where(x => x.OrderID == id), "Order", cancellationToken);
        var allowed = (order.Status, request.Status) switch { (OrderStatus.Draft, OrderStatus.Cancelled) => true, (OrderStatus.Submitted, OrderStatus.Cancelled) => true, (OrderStatus.UnderReview, OrderStatus.Rejected) => true, (OrderStatus.Delivered, OrderStatus.Closed) => true, (OrderStatus.Returned, OrderStatus.Closed) => true, _ => false };
        if (!allowed) throw new BusinessRuleException(BusinessRuleMessages.InvalidStatusTransition);
        order.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateRegularAsync(SaveRegularOrderRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.OrderNo) || string.IsNullOrWhiteSpace(request.DeliveryAddressSnapshot)) throw new BusinessRuleException("Order number and delivery address are required.");
        OrderFulfilmentServiceHelper.ValidateOrderItems(request.Items, x => x.SKUID, x => x.OrderedQuantity);
        if (request.Items.Any(x => x.FreeQuantity < 0 || x.UnitPrice < 0 || x.DiscountPercent is < 0 or > 100 || x.TaxAmount < 0)) throw new BusinessRuleException("Order item values are invalid.");
        await EnsureOrderNumberUniqueAsync(request.OrderNo, null, cancellationToken);
        if (!await _db.Clients.AnyAsync(x => x.ClientID == request.ClientID && x.IsActive, cancellationToken)) throw new NotFoundException("Active client was not found.");
        if (request.EmployeeID.HasValue && !await _db.Employees.AnyAsync(x => x.EmployeeID == request.EmployeeID, cancellationToken)) throw new NotFoundException("Employee was not found.");
        if (request.CampaignID.HasValue && !await _db.Campaigns.AnyAsync(x => x.CampaignID == request.CampaignID && x.Status == CampaignStatus.Active, cancellationToken)) throw new BusinessRuleException("An active campaign is required.");
        var skuIDs = request.Items.Select(x => x.SKUID).Distinct().ToArray(); if (await _db.SKUs.CountAsync(x => skuIDs.Contains(x.SKUID) && x.IsActive && x.Product.IsActive, cancellationToken) != skuIDs.Length) throw new NotFoundException("One or more active SKUs were not found.");
    }

    private async Task EnsureOrderNumberUniqueAsync(string number, int? id, CancellationToken cancellationToken) { var code = number.Trim().ToUpperInvariant(); if (await _db.Orders.AnyAsync(x => x.OrderNo == code && x.OrderID != id, cancellationToken)) throw new ConflictException("Order number already exists."); }
    private static void ApplyRegular(Order order, SaveRegularOrderRequestDto request) { order.OrderNo = request.OrderNo.Trim().ToUpperInvariant(); order.ClientID = request.ClientID; order.EmployeeID = request.EmployeeID; order.Channel = request.Channel; order.CampaignID = request.CampaignID; order.PriceListID = request.PriceListID; order.OrderDate = request.OrderDate; order.RequestedDeliveryDate = request.RequestedDeliveryDate; order.DeliveryAddressSnapshot = request.DeliveryAddressSnapshot.Trim(); order.GrossAmount = request.Items.Sum(x => x.OrderedQuantity * x.UnitPrice); order.DiscountAmount = request.Items.Sum(x => x.OrderedQuantity * x.UnitPrice * x.DiscountPercent / 100m); order.TaxAmount = request.Items.Sum(x => x.TaxAmount); order.NetAmount = order.GrossAmount - order.DiscountAmount + order.TaxAmount; }
    private static OrderItem ToOrderItem(Order order, SaveOrderItemRequestDto input) { var gross = input.OrderedQuantity * input.UnitPrice; var discount = gross * input.DiscountPercent / 100m; return new() { Order = order, SKUID = input.SKUID, OrderedQuantity = input.OrderedQuantity, FreeQuantity = input.FreeQuantity, UnitPrice = input.UnitPrice, DiscountPercent = input.DiscountPercent, DiscountAmount = discount, TaxAmount = input.TaxAmount, LineTotal = gross - discount + input.TaxAmount, ApprovedQuantity = input.OrderedQuantity + input.FreeQuantity, BackorderQuantity = input.OrderedQuantity + input.FreeQuantity }; }
}

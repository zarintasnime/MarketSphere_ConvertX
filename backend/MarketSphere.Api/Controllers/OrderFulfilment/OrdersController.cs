using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
using MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.OrderFulfilment;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly IAppliedOfferService _offerService;

    public OrdersController(
        IOrderService service,
        IAppliedOfferService offerService)
    {
        _service = service;
        _offerService = offerService;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.OrdersView)]
    public async Task<ActionResult<ApiResponse<PagedResult<OrderListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<OrderListDto>>.Success(result, "Orders retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.OrdersView)]
    public async Task<ActionResult<ApiResponse<OrderDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<OrderDetailsDto>.Success(result, "Order retrieved successfully."));
    }

    [HttpPost("regular")]
    [HasPermission(PermissionCodes.OrdersManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateRegular(
        [FromBody] SaveRegularOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateRegularAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Regular order created successfully."));
    }

    [HttpPost("from-quotation")]
    [HasPermission(PermissionCodes.OrdersManage)]
    public async Task<ActionResult<ApiResponse<int>>> ConvertQuotation(
        [FromBody] ConvertQuotationToOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.ConvertQuotationAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Quotation converted to order successfully."));
    }

    [HttpPost("from-mt-purchase-order")]
    [HasPermission(PermissionCodes.OrdersManage)]
    public async Task<ActionResult<ApiResponse<int>>> ConvertModernTradePurchaseOrder(
        [FromBody] ConvertModernTradePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.ConvertModernTradePurchaseOrderAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Modern trade purchase order converted successfully."));
    }

    [HttpPost("{id:int}/submit")]
    [HasPermission(PermissionCodes.OrdersSubmit)]
    public async Task<ActionResult<ApiResponse<bool>>> Submit(
        int id,
        CancellationToken cancellationToken)
    {
        await _service.SubmitAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Order submitted successfully."));
    }

    [HttpPost("{id:int}/approve-and-reserve")]
    [HasPermission(PermissionCodes.OrdersApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ApproveAndReserve(
        int id,
        [FromBody] ApproveAndReserveOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ApproveAndReserveAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Order approved and stock reserved successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.OrdersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeOrderStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Order status changed successfully."));
    }

    [HttpGet("{orderID:int}/applied-offers")]
    [HasPermission(PermissionCodes.OrdersView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AppliedOfferDto>>>> GetAppliedOffers(
            int orderID,
            CancellationToken cancellationToken)
    {
        var result = await _offerService.GetForOrderAsync(orderID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<AppliedOfferDto>>.Success(result, "Applied offers retrieved successfully."));
    }

    [HttpPost("applied-offers")]
    [HasPermission(PermissionCodes.AppliedOffersManage)]
    public async Task<ActionResult<ApiResponse<int>>> ApplyOffer(
        [FromBody] ApplyOfferRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _offerService.ApplyAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Offer applied successfully."));
    }

    [HttpDelete("applied-offers/{id:int}")]
    [HasPermission(PermissionCodes.AppliedOffersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveOffer(
        int id,
        CancellationToken cancellationToken)
    {
        await _offerService.RemoveAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Applied offer removed successfully."));
    }
}

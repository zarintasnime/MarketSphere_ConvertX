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
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.PaymentsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<PaymentListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PaymentListDto>>.Success(result, "Payments retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.PaymentsView)]
    public async Task<ActionResult<ApiResponse<PaymentDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PaymentDetailsDto>.Success(result, "Payment retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.PaymentsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreatePaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Payment created successfully."));
    }

    [HttpPost("{id:int}/confirm")]
    [HasPermission(PermissionCodes.PaymentsAllocate)]
    public async Task<ActionResult<ApiResponse<bool>>> Confirm(
        int id,
        [FromBody] ConfirmPaymentRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ConfirmAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Payment confirmed successfully."));
    }

    [HttpPost("allocations/reverse")]
    [HasPermission(PermissionCodes.PaymentsReverse)]
    public async Task<ActionResult<ApiResponse<bool>>> ReverseAllocation(
        [FromBody] ReversePaymentAllocationRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ReverseAllocationAsync(request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Payment allocation reversed successfully."));
    }
}

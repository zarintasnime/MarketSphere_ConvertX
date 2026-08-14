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
[Route("api/deliveries")]
public sealed class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _service;

    public DeliveriesController(IDeliveryService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.DeliveriesView)]
    public async Task<ActionResult<ApiResponse<PagedResult<DeliveryListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<DeliveryListDto>>.Success(result, "Deliveries retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.DeliveriesView)]
    public async Task<ActionResult<ApiResponse<DeliveryDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DeliveryDetailsDto>.Success(result, "Delivery retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.DeliveriesManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Delivery created successfully."));
    }

    [HttpPost("{id:int}/dispatch")]
    [HasPermission(PermissionCodes.DeliveriesDispatch)]
    public async Task<ActionResult<ApiResponse<bool>>> Dispatch(
        int id,
        [FromBody] DispatchDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.DispatchAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Delivery dispatched successfully."));
    }

    [HttpPost("{id:int}/complete")]
    [HasPermission(PermissionCodes.DeliveriesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Complete(
        int id,
        [FromBody] CompleteDeliveryRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.CompleteAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Delivery completed successfully."));
    }
}

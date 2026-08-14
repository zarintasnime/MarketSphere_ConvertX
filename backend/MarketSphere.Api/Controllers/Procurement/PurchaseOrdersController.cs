using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
using MarketSphere.Application.Modules.Procurement.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.Procurement;

[ApiController]
[Authorize]
[Route("api/purchase-orders")]
public sealed class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _service;

    public PurchaseOrdersController(IPurchaseOrderService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.PurchaseOrdersView)]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PurchaseOrderListDto>>.Success(result, "Purchase order records retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.PurchaseOrdersView)]
    public async Task<ActionResult<ApiResponse<PurchaseOrderDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PurchaseOrderDetailsDto>.Success(result, "Purchase order retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.PurchaseOrdersManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SavePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Purchase order created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.PurchaseOrdersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SavePurchaseOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Purchase order updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.PurchaseOrdersApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangePurchaseOrderStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Purchase order status changed successfully."));
    }
}

using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.DTOs;
using MarketSphere.Application.Modules.Inventory.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.Inventory;

[ApiController]
[Authorize]
[Route("api/stock-adjustments")]
public sealed class StockAdjustmentsController : ControllerBase
{
    private readonly IStockAdjustmentService _service;

    public StockAdjustmentsController(IStockAdjustmentService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.StockAdjustmentsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<StockAdjustmentListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<StockAdjustmentListDto>>.Success(result, "Stock adjustment records retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.StockAdjustmentsView)]
    public async Task<ActionResult<ApiResponse<StockAdjustmentDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<StockAdjustmentDetailsDto>.Success(result, "Stock adjustment retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.StockAdjustmentsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveStockAdjustmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Stock adjustment created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.StockAdjustmentsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveStockAdjustmentRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock adjustment updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.StockAdjustmentsApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeStockAdjustmentStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock adjustment status changed successfully."));
    }

    [HttpPost("{id:int}/post")]
    [HasPermission(PermissionCodes.StockAdjustmentsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Post(
        int id,
        [FromBody] PostStockAdjustmentRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.PostAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock adjustment posted successfully."));
    }
}

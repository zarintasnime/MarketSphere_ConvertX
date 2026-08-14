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
[Route("api/stock-transfers")]
public sealed class StockTransfersController : ControllerBase
{
    private readonly IStockTransferService _service;

    public StockTransfersController(IStockTransferService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.StockTransfersView)]
    public async Task<ActionResult<ApiResponse<PagedResult<StockTransferListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<StockTransferListDto>>.Success(result, "Stock transfers retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.StockTransfersView)]
    public async Task<ActionResult<ApiResponse<StockTransferDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<StockTransferDetailsDto>.Success(result, "Stock transfer retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.StockTransfersManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveStockTransferRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Stock transfer created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.StockTransfersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveStockTransferRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock transfer updated successfully."));
    }

    [HttpPost("{id:int}/submit")]
    [HasPermission(PermissionCodes.StockTransfersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Submit(
        int id,
        CancellationToken cancellationToken)
    {
        await _service.SubmitAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock transfer submitted successfully."));
    }

    [HttpPost("{id:int}/approve")]
    [HasPermission(PermissionCodes.StockTransfersApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> Approve(
        int id,
        CancellationToken cancellationToken)
    {
        await _service.ApproveAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock transfer approved successfully."));
    }

    [HttpPost("{id:int}/dispatch")]
    [HasPermission(PermissionCodes.StockTransfersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Dispatch(
        int id,
        [FromBody] DispatchStockTransferRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.DispatchAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock transfer dispatched successfully."));
    }

    [HttpPost("{id:int}/receive")]
    [HasPermission(PermissionCodes.StockTransfersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Receive(
        int id,
        [FromBody] ReceiveStockTransferRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ReceiveAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Stock transfer received successfully."));
    }
}

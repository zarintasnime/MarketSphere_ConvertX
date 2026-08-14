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
[Route("api/stock")]
public sealed class StockController : ControllerBase
{
    private readonly IStockService _service;

    public StockController(IStockService service) => _service = service;

    [HttpGet("balances")]
    [HasPermission(PermissionCodes.StockView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<StockBalanceDto>>>> GetBalances(
            [FromQuery] StockSearchRequestDto request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetBalancesAsync(request, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<StockBalanceDto>>.Success(result, "Stock balances retrieved successfully."));
    }

    [HttpGet("batches")]
    [HasPermission(PermissionCodes.StockView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<BatchDto>>>> GetBatches(
            [FromQuery] int? skuID,
            [FromQuery] bool includeExpired = false,
            CancellationToken cancellationToken = default)
    {
        var result = await _service.GetBatchesAsync(skuID, includeExpired, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<BatchDto>>.Success(result, "Batches retrieved successfully."));
    }

    [HttpGet("movements")]
    [HasPermission(PermissionCodes.StockMovementsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<StockMovementDto>>>> GetMovements(
            [FromQuery] int? warehouseID,
            [FromQuery] int? skuID,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetMovementsAsync(warehouseID, skuID, from, to, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<StockMovementDto>>.Success(result, "Stock movements retrieved successfully."));
    }

    [HttpGet("reservations")]
    [HasPermission(PermissionCodes.StockView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<StockReservationDto>>>> GetReservations(
            [FromQuery] int? orderItemID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetReservationsAsync(orderItemID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<StockReservationDto>>.Success(result, "Stock reservations retrieved successfully."));
    }
}

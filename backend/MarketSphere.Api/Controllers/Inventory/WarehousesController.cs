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
[Route("api/warehouses")]
public sealed class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _service;

    public WarehousesController(IWarehouseService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.WarehousesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<WarehouseDto>>>> Get(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<WarehouseDto>>.Success(result, "Warehouses retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.WarehousesManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveWarehouseRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Warehouse created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.WarehousesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveWarehouseRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Warehouse updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.WarehousesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeWarehouseStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Warehouse status changed successfully."));
    }
}

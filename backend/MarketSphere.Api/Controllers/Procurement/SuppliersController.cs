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
[Route("api/suppliers")]
public sealed class SuppliersController : ControllerBase
{
    private readonly ISupplierService _service;

    public SuppliersController(ISupplierService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.SuppliersView)]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplierListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<SupplierListDto>>.Success(result, "Suppliers retrieved successfully."));
    }

    [HttpGet("{supplierID:int}")]
    [HasPermission(PermissionCodes.SuppliersView)]
    public async Task<ActionResult<ApiResponse<SupplierDetailsDto>>> GetById(
            int supplierID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(supplierID, cancellationToken);
        return Ok(ApiResponse<SupplierDetailsDto>.Success(result, "Supplier retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.SuppliersManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveSupplierRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Supplier created successfully."));
    }

    [HttpPut("{supplierID:int}")]
    [HasPermission(PermissionCodes.SuppliersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int supplierID,
        [FromBody] SaveSupplierRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(supplierID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Supplier updated successfully."));
    }

    [HttpPatch("{supplierID:int}/status")]
    [HasPermission(PermissionCodes.SuppliersManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int supplierID,
        [FromBody] ChangeSupplierStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(supplierID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Supplier status changed successfully."));
    }

    [HttpPost("{supplierID:int}/products")]
    [HasPermission(PermissionCodes.SuppliersManage)]
    public async Task<ActionResult<ApiResponse<int>>> UpsertProduct(
        int supplierID,
        [FromBody] SaveSupplierProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.UpsertProductAsync(supplierID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Supplier product saved successfully."));
    }
}

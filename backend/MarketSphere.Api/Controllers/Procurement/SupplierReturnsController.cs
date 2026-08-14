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
[Route("api/supplier-returns")]
public sealed class SupplierReturnsController : ControllerBase
{
    private readonly ISupplierReturnService _service;

    public SupplierReturnsController(ISupplierReturnService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.SupplierReturnsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplierReturnListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<SupplierReturnListDto>>.Success(result, "Supplier return records retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.SupplierReturnsView)]
    public async Task<ActionResult<ApiResponse<SupplierReturnDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<SupplierReturnDetailsDto>.Success(result, "Supplier return retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.SupplierReturnsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveSupplierReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Supplier return created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.SupplierReturnsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveSupplierReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Supplier return updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.SupplierReturnsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeSupplierReturnStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Supplier return status changed successfully."));
    }

    [HttpPost("{id:int}/post")]
    [HasPermission(PermissionCodes.SupplierReturnsPost)]
    public async Task<ActionResult<ApiResponse<bool>>> Post(
        int id,
        [FromBody] PostSupplierReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.PostAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Supplier return posted successfully."));
    }
}

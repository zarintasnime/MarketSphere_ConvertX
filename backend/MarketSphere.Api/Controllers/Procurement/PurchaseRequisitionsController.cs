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
[Route("api/purchase-requisitions")]
public sealed class PurchaseRequisitionsController : ControllerBase
{
    private readonly IPurchaseRequisitionService _service;

    public PurchaseRequisitionsController(IPurchaseRequisitionService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.PurchaseRequisitionsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseRequisitionListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PurchaseRequisitionListDto>>.Success(result, "Purchase requisition records retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.PurchaseRequisitionsView)]
    public async Task<ActionResult<ApiResponse<PurchaseRequisitionDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PurchaseRequisitionDetailsDto>.Success(result, "Purchase requisition retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.PurchaseRequisitionsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SavePurchaseRequisitionRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Purchase requisition created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.PurchaseRequisitionsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SavePurchaseRequisitionRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Purchase requisition updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.PurchaseRequisitionsApprove)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangePurchaseRequisitionStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Purchase requisition status changed successfully."));
    }
}

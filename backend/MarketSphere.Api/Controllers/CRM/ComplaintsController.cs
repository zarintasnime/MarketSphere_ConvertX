using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
using MarketSphere.Application.Modules.CRM.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.CRM;

[ApiController]
[Authorize]
[Route("api/complaints")]
public sealed class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _service;

    public ComplaintsController(IComplaintService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.ComplaintsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ComplaintListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] bool slaBreachedOnly = false,
            CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPagedAsync(request, slaBreachedOnly, cancellationToken);
        return Ok(ApiResponse<PagedResult<ComplaintListDto>>.Success(result, "Complaints retrieved successfully."));
    }

    [HttpGet("{complaintID:int}")]
    [HasPermission(PermissionCodes.ComplaintsView)]
    public async Task<ActionResult<ApiResponse<ComplaintDetailsDto>>> GetById(
            int complaintID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(complaintID, cancellationToken);
        return Ok(ApiResponse<ComplaintDetailsDto>.Success(result, "Complaint retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ComplaintsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveComplaintRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Complaint created successfully."));
    }

    [HttpPut("{complaintID:int}")]
    [HasPermission(PermissionCodes.ComplaintsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int complaintID,
        [FromBody] SaveComplaintRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(complaintID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Complaint updated successfully."));
    }

    [HttpPatch("{complaintID:int}/status")]
    [HasPermission(PermissionCodes.ComplaintsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int complaintID,
        [FromBody] ChangeComplaintStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(complaintID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Complaint status changed successfully."));
    }
}

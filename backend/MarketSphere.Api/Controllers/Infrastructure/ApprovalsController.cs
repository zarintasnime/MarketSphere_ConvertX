using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.Infrastructure;

[ApiController]
[Authorize]
[Route("api/approvals")]
public sealed class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _service;

    public ApprovalsController(IApprovalService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.ApprovalsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ApprovalRequestDto>>>> GetQueue(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetQueueAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ApprovalRequestDto>>.Success(result, "Approval queue retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.ApprovalsView)]
    public async Task<ActionResult<ApiResponse<ApprovalRequestDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ApprovalRequestDto>.Success(result, "Approval request retrieved successfully."));
    }

    [HttpGet("policies")]
    [HasPermission(PermissionCodes.ApprovalsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ApprovalPolicyDto>>>> GetPolicies(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPoliciesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ApprovalPolicyDto>>.Success(result, "Approval policies retrieved successfully."));
    }

    [HttpPost("policies")]
    [HasPermission(PermissionCodes.ApprovalsManage)]
    public async Task<ActionResult<ApiResponse<int>>> SavePolicy(
        [FromBody] SaveApprovalPolicyRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.SavePolicyAsync(null, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Approval policy created successfully."));
    }

    [HttpPut("policies/{id:int}")]
    [HasPermission(PermissionCodes.ApprovalsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePolicy(
        int id,
        [FromBody] SaveApprovalPolicyRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SavePolicyAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Approval policy updated successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ApprovalsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateRequest(
        [FromBody] CreateApprovalRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateRequestAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Approval request created successfully."));
    }

    [HttpPost("{id:int}/actions")]
    [HasPermission(PermissionCodes.ApprovalsAct)]
    public async Task<ActionResult<ApiResponse<bool>>> Act(
        int id,
        [FromBody] ApprovalActionRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ActAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Approval action recorded successfully."));
    }

    [HttpPost("{id:int}/cancel")]
    [HasPermission(PermissionCodes.ApprovalsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(
        int id,
        [FromQuery] string? note,
        CancellationToken cancellationToken)
    {
        await _service.CancelAsync(id, note, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Approval request cancelled successfully."));
    }
}

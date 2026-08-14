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
[Route("api/leads")]
public sealed class LeadsController : ControllerBase
{
    private readonly ILeadService _service;

    public LeadsController(ILeadService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.LeadsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<LeadListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<LeadListDto>>.Success(result, "Leads retrieved successfully."));
    }

    [HttpGet("{leadID:int}")]
    [HasPermission(PermissionCodes.LeadsView)]
    public async Task<ActionResult<ApiResponse<LeadDetailsDto>>> GetById(
            int leadID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(leadID, cancellationToken);
        return Ok(ApiResponse<LeadDetailsDto>.Success(result, "Lead retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.LeadsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveLeadRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Lead created successfully."));
    }

    [HttpPut("{leadID:int}")]
    [HasPermission(PermissionCodes.LeadsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int leadID,
        [FromBody] SaveLeadRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(leadID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Lead updated successfully."));
    }

    [HttpPatch("{leadID:int}/status")]
    [HasPermission(PermissionCodes.LeadsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int leadID,
        [FromBody] ChangeLeadStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(leadID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Lead status changed successfully."));
    }

    [HttpPost("{leadID:int}/score/recalculate")]
    [HasPermission(PermissionCodes.LeadsManage)]
    public async Task<ActionResult<ApiResponse<LeadScoreResultDto>>> RecalculateScore(
            int leadID,
            CancellationToken cancellationToken)
    {
        var result = await _service.RecalculateScoreAsync(leadID, cancellationToken);
        return Ok(ApiResponse<LeadScoreResultDto>.Success(result, "Lead score recalculated successfully."));
    }

    [HttpGet("{leadID:int}/duplicates")]
    [HasPermission(PermissionCodes.LeadsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DuplicateCandidateDto>>>> FindDuplicates(
            int leadID,
            CancellationToken cancellationToken)
    {
        var result = await _service.FindDuplicatesAsync(leadID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<DuplicateCandidateDto>>.Success(result, "Duplicate candidates retrieved successfully."));
    }

    [HttpGet("duplicate-reviews")]
    [HasPermission(PermissionCodes.DuplicateReviewsManage)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<DuplicateReviewDto>>>> GetDuplicateReviews(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetDuplicateReviewsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<DuplicateReviewDto>>.Success(result, "Duplicate reviews retrieved successfully."));
    }

    [HttpPost("duplicate-reviews/{duplicateReviewCaseID:int}/resolve")]
    [HasPermission(PermissionCodes.DuplicateReviewsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ResolveDuplicateReview(
        int duplicateReviewCaseID,
        [FromBody] ResolveDuplicateReviewRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ResolveDuplicateReviewAsync(duplicateReviewCaseID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Duplicate review resolved successfully."));
    }

    [HttpPost("score-rules")]
    [HasPermission(PermissionCodes.LeadScoreRulesManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateScoreRule(
        [FromBody] SaveLeadScoreRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateScoreRuleAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Lead score rule created successfully."));
    }

    [HttpPost("{leadID:int}/convert-to-client")]
    [HasPermission(PermissionCodes.LeadsManage)]
    public async Task<ActionResult<ApiResponse<int>>> ConvertToClient(
        int leadID,
        [FromBody] ConvertLeadToClientRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.ConvertToClientAsync(leadID, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Lead converted to client successfully."));
    }
}

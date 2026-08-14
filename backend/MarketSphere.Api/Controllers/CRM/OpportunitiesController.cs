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
[Route("api/opportunities")]
public sealed class OpportunitiesController : ControllerBase
{
    private readonly IOpportunityService _service;

    public OpportunitiesController(IOpportunityService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.OpportunitiesView)]
    public async Task<ActionResult<ApiResponse<PagedResult<OpportunityListDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<OpportunityListDto>>.Success(result, "Opportunities retrieved successfully."));
    }

    [HttpGet("{opportunityID:int}")]
    [HasPermission(PermissionCodes.OpportunitiesView)]
    public async Task<ActionResult<ApiResponse<OpportunityDetailsDto>>> GetById(
            int opportunityID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(opportunityID, cancellationToken);
        return Ok(ApiResponse<OpportunityDetailsDto>.Success(result, "Opportunity retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.OpportunitiesManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveOpportunityRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Opportunity created successfully."));
    }

    [HttpPut("{opportunityID:int}")]
    [HasPermission(PermissionCodes.OpportunitiesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int opportunityID,
        [FromBody] SaveOpportunityRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(opportunityID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Opportunity updated successfully."));
    }

    [HttpPatch("{opportunityID:int}/stage")]
    [HasPermission(PermissionCodes.OpportunitiesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStage(
        int opportunityID,
        [FromBody] ChangeOpportunityStageRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStageAsync(opportunityID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Opportunity stage changed successfully."));
    }
}

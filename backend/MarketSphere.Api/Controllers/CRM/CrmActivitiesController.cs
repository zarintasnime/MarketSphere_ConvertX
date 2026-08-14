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
[Route("api/crm-activities")]
public sealed class CrmActivitiesController : ControllerBase
{
    private readonly ICrmActivityService _service;

    public CrmActivitiesController(ICrmActivityService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.ActivitiesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CrmActivityDto>>>> GetTimeline(
            [FromQuery] int? leadID,
            [FromQuery] int? clientID,
            [FromQuery] int? opportunityID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetTimelineAsync(leadID, clientID, opportunityID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<CrmActivityDto>>.Success(result, "CRM timeline retrieved successfully."));
    }

    [HttpGet("{activityID:int}")]
    [HasPermission(PermissionCodes.ActivitiesView)]
    public async Task<ActionResult<ApiResponse<CrmActivityDto>>> GetById(
            int activityID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(activityID, cancellationToken);
        return Ok(ApiResponse<CrmActivityDto>.Success(result, "CRM activity retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ActivitiesManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveCrmActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "CRM activity created successfully."));
    }

    [HttpPut("{activityID:int}")]
    [HasPermission(PermissionCodes.ActivitiesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int activityID,
        [FromBody] SaveCrmActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(activityID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "CRM activity updated successfully."));
    }
}

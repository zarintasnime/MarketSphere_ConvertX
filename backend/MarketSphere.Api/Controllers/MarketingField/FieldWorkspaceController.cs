using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;
using MarketSphere.Application.Modules.MarketingField.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.MarketingField;

[ApiController]
[Authorize]
[Route("api/field-workspace")]
public sealed class FieldWorkspaceController : ControllerBase
{
    private readonly IFieldWorkspaceService _service;

    public FieldWorkspaceController(IFieldWorkspaceService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    [HasPermission(PermissionCodes.VisitsView)]
    public async Task<ActionResult<ApiResponse<FieldWorkspaceSummaryDto>>> GetSummary(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetSummaryAsync(cancellationToken);

        return Ok(ApiResponse<FieldWorkspaceSummaryDto>.Success(
            result,
            "Field workspace summary retrieved successfully."));
    }

    [HttpGet("assigned-clients")]
    [HasPermission(PermissionCodes.VisitsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<FieldAssignedClientDto>>>> GetAssignedClients(
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAssignedClientsAsync(
            request,
            cancellationToken);

        return Ok(ApiResponse<PagedResult<FieldAssignedClientDto>>.Success(
            result,
            "Assigned clients retrieved successfully."));
    }

    [HttpGet("my-visits")]
    [HasPermission(PermissionCodes.VisitsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<FieldVisitListDto>>>> GetMyVisits(
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetMyVisitsAsync(
            request,
            cancellationToken);

        return Ok(ApiResponse<PagedResult<FieldVisitListDto>>.Success(
            result,
            "Employee visits retrieved successfully."));
    }

    [HttpGet("active-visit")]
    [HasPermission(PermissionCodes.VisitsView)]
    public async Task<ActionResult<ApiResponse<FieldActiveVisitDto?>>> GetActiveVisit(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetActiveVisitAsync(cancellationToken);

        return Ok(ApiResponse<FieldActiveVisitDto?>.Success(
            result,
            result is null
                ? "No active visit was found."
                : "Active visit retrieved successfully."));
    }
}
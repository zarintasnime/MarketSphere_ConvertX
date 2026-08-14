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
[Route("api/reactivation")]
public sealed class ReactivationController : ControllerBase
{
    private readonly IReactivationService _service;

    public ReactivationController(IReactivationService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.ReactivationView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ReactivationCaseDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ReactivationCaseDto>>.Success(result, "Reactivation cases retrieved successfully."));
    }

    [HttpGet("{reactivationCaseID:int}")]
    [HasPermission(PermissionCodes.ReactivationView)]
    public async Task<ActionResult<ApiResponse<ReactivationCaseDto>>> GetById(
            int reactivationCaseID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(reactivationCaseID, cancellationToken);
        return Ok(ApiResponse<ReactivationCaseDto>.Success(result, "Reactivation case retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ReactivationManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateReactivationCaseRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Reactivation case created successfully."));
    }

    [HttpPost("{reactivationCaseID:int}/resolve")]
    [HasPermission(PermissionCodes.ReactivationManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Resolve(
        int reactivationCaseID,
        [FromBody] ResolveReactivationCaseRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ResolveAsync(reactivationCaseID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Reactivation case resolved successfully."));
    }
}

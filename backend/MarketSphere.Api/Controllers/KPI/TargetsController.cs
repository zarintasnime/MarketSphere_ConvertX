using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;
using MarketSphere.Application.Modules.KPI.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.KPI;

[ApiController]
[Authorize]
[Route("api/targets")]
public sealed class TargetsController : ControllerBase
{
    private readonly ITargetService _service;

    public TargetsController(ITargetService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.TargetsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeTargetListDto>>>> Get(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<EmployeeTargetListDto>>.Success(result, "Employee targets retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.TargetsView)]
    public async Task<ActionResult<ApiResponse<EmployeeTargetDetailsDto>>> GetById(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<EmployeeTargetDetailsDto>.Success(result, "Employee target retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.TargetsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveEmployeeTargetRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Employee target created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.TargetsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveEmployeeTargetRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Employee target updated successfully."));
    }

    [HttpPatch("{id:int}/status")]
    [HasPermission(PermissionCodes.TargetsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int id,
        [FromBody] ChangeEmployeeTargetStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Employee target status changed successfully."));
    }

    [HttpGet("{id:int}/progress")]
    [HasPermission(PermissionCodes.TargetsView)]
    public async Task<ActionResult<ApiResponse<TargetProgressDto>>> GetProgress(
            int id,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetProgressAsync(id, cancellationToken);
        return Ok(ApiResponse<TargetProgressDto>.Success(result, "Target progress retrieved successfully."));
    }
}

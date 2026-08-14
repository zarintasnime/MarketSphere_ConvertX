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
[Route("api/crm-tasks")]
public sealed class CrmTasksController : ControllerBase
{
    private readonly ICrmTaskService _service;

    public CrmTasksController(ICrmTaskService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.TasksView)]
    public async Task<ActionResult<ApiResponse<PagedResult<CrmTaskDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            [FromQuery] int? assignedEmployeeID,
            [FromQuery] bool overdueOnly = false,
            CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPagedAsync(request, assignedEmployeeID, overdueOnly, cancellationToken);
        return Ok(ApiResponse<PagedResult<CrmTaskDto>>.Success(result, "CRM tasks retrieved successfully."));
    }

    [HttpGet("{taskID:int}")]
    [HasPermission(PermissionCodes.TasksView)]
    public async Task<ActionResult<ApiResponse<CrmTaskDto>>> GetById(
            int taskID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(taskID, cancellationToken);
        return Ok(ApiResponse<CrmTaskDto>.Success(result, "CRM task retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.TasksManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveCrmTaskRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "CRM task created successfully."));
    }

    [HttpPut("{taskID:int}")]
    [HasPermission(PermissionCodes.TasksManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int taskID,
        [FromBody] SaveCrmTaskRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(taskID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "CRM task updated successfully."));
    }

    [HttpPatch("{taskID:int}/status")]
    [HasPermission(PermissionCodes.TasksManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int taskID,
        [FromBody] ChangeCrmTaskStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(taskID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "CRM task status changed successfully."));
    }
}

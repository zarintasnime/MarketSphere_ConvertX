using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.OrganizationSecurity;

[ApiController]
[Authorize]
[Route("api/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.EmployeesView)]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeListItemDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<EmployeeListItemDto>>.Success(result, "Employees retrieved successfully."));
    }

    [HttpGet("{employeeID:int}")]
    [HasPermission(PermissionCodes.EmployeesView)]
    public async Task<ActionResult<ApiResponse<EmployeeDetailsDto>>> GetByID(
            int employeeID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIDAsync(employeeID, cancellationToken);
        return Ok(ApiResponse<EmployeeDetailsDto>.Success(result, "Employee retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.EmployeesCreate)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateEmployeeRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Employee created successfully."));
    }

    [HttpPut("{employeeID:int}")]
    [HasPermission(PermissionCodes.EmployeesUpdate)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int employeeID,
        [FromBody] UpdateEmployeeRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(employeeID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Employee updated successfully."));
    }
}

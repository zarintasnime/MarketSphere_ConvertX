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
[Route("api/roles")]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _service;

    public RolesController(IRoleService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.RolesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RoleListItemDto>>>> GetAll(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RoleListItemDto>>.Success(result, "Roles retrieved successfully."));
    }

    [HttpGet("{roleID:int}")]
    [HasPermission(PermissionCodes.RolesView)]
    public async Task<ActionResult<ApiResponse<RoleDetailsDto>>> GetByID(
            int roleID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIDAsync(roleID, cancellationToken);
        return Ok(ApiResponse<RoleDetailsDto>.Success(result, "Role retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.RolesCreate)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Role created successfully."));
    }

    [HttpPut("{roleID:int}")]
    [HasPermission(PermissionCodes.RolesUpdate)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int roleID,
        [FromBody] UpdateRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(roleID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Role updated successfully."));
    }

    [HttpPut("{roleID:int}/permissions")]
    [HasPermission(PermissionCodes.RolesManagePermissions)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePermissions(
        int roleID,
        [FromBody] UpdateRolePermissionsRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdatePermissionsAsync(roleID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Role permissions updated successfully."));
    }
}

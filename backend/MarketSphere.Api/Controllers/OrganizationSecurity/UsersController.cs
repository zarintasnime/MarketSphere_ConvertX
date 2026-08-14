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
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.UsersView)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListItemDto>>>> GetPaged(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<UserListItemDto>>.Success(result, "Users retrieved successfully."));
    }

    [HttpGet("{userID:int}")]
    [HasPermission(PermissionCodes.UsersView)]
    public async Task<ActionResult<ApiResponse<UserDetailsDto>>> GetByID(
            int userID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByIDAsync(userID, cancellationToken);
        return Ok(ApiResponse<UserDetailsDto>.Success(result, "User retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.UsersCreate)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "User created successfully."));
    }

    [HttpPut("{userID:int}")]
    [HasPermission(PermissionCodes.UsersUpdate)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int userID,
        [FromBody] UpdateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(userID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "User updated successfully."));
    }

    [HttpPatch("{userID:int}/status")]
    [HasPermission(PermissionCodes.UsersChangeStatus)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeStatus(
        int userID,
        [FromBody] ChangeUserStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangeStatusAsync(userID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "User status changed successfully."));
    }

    [HttpPut("{userID:int}/roles")]
    [HasPermission(PermissionCodes.UsersAssignRoles)]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoles(
        int userID,
        [FromBody] AssignUserRolesRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.AssignRolesAsync(userID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "User roles updated successfully."));
    }

    [HttpPost("{userID:int}/account-token")]
    [HasPermission(PermissionCodes.UsersCreateToken)]
    public async Task<ActionResult<ApiResponse<AccountTokenResultDto>>> CreateAccountToken(
            int userID,
            [FromQuery] MarketSphere.Domain.Enums.AccountTokenType tokenType,
            CancellationToken cancellationToken)
    {
        var result = await _service.CreateAccountTokenAsync(userID, tokenType, cancellationToken);
        return Ok(ApiResponse<AccountTokenResultDto>.Success(result, "Account token created successfully."));
    }
}

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
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service) => _service = service;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthSessionDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _service.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthSessionDto>.Success(result, "Login successful."));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthSessionDto>>> RefreshSession(
        [FromBody] RefreshSessionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _service.RefreshSessionAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthSessionDto>.Success(result, "Session refreshed successfully."));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangePasswordAsync(request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Password changed successfully."));
    }

    [HttpPost("activate-account")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> ActivateAccount(
        [FromBody] ActivateAccountRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ActivateAccountAsync(request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Account activated successfully."));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(
        [FromBody] ResetPasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ResetPasswordAsync(request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Password reset successfully."));
    }

    [HttpPost("sessions/{userSessionID:int}/revoke")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeSession(
        int userSessionID,
        CancellationToken cancellationToken)
    {
        await _service.RevokeSessionAsync(userSessionID, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Session revoked successfully."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<AuthenticatedUserDto>>> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetCurrentUserAsync(cancellationToken);
        return Ok(ApiResponse<AuthenticatedUserDto>.Success(result, "Current user retrieved successfully."));
    }
}

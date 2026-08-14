using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.Infrastructure;

[ApiController]
[Authorize]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly ISettingService _service;

    public SettingsController(ISettingService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.SettingsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<SystemSettingDto>>>> Get(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<SystemSettingDto>>.Success(result, "System settings retrieved successfully."));
    }

    [HttpGet("by-key")]
    [HasPermission(PermissionCodes.SettingsView)]
    public async Task<ActionResult<ApiResponse<SystemSettingDto?>>> GetByKey(
            [FromQuery] string key,
            [FromQuery] int? scopeID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetByKeyAsync(key, scopeID, cancellationToken);
        return Ok(ApiResponse<SystemSettingDto?>.Success(result, "System setting retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] SaveSystemSettingRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.SaveAsync(null, request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "System setting created successfully."));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.SettingsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int id,
        [FromBody] SaveSystemSettingRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SaveAsync(id, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "System setting updated successfully."));
    }
}

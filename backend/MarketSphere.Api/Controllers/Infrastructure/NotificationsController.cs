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
[Route("api/notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.NotificationsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationDto>>>> GetMine(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetMineAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<NotificationDto>>.Success(result, "Notifications retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.NotificationsManage)]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Notification created successfully."));
    }

    [HttpPost("{id:int}/read")]
    [HasPermission(PermissionCodes.NotificationsView)]
    public async Task<ActionResult<ApiResponse<bool>>> MarkRead(
        int id,
        CancellationToken cancellationToken)
    {
        await _service.MarkReadAsync(id, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Notification marked as read."));
    }

    [HttpPost("read-all")]
    [HasPermission(PermissionCodes.NotificationsView)]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAllRead(
        CancellationToken cancellationToken)
    {
        await _service.MarkAllReadAsync(cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "All notifications marked as read."));
    }
}

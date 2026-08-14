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
[Route("api/audit")]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditService _service;

    public AuditController(IAuditService service) => _service = service;

    [HttpGet("logs")]
    [HasPermission(PermissionCodes.AuditLogsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditLogDto>>>> GetAuditLogs(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAuditLogsAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<AuditLogDto>>.Success(result, "Audit logs retrieved successfully."));
    }

    [HttpGet("status-history")]
    [HasPermission(PermissionCodes.AuditLogsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<StatusHistoryDto>>>> GetStatusHistory(
            [FromQuery] string entityType,
            [FromQuery] int entityID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetStatusHistoryAsync(entityType, entityID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<StatusHistoryDto>>.Success(result, "Status history retrieved successfully."));
    }
}

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
[Route("api/system-checks")]
public sealed class SystemChecksController : ControllerBase
{
    private readonly ISystemCheckService _service;

    public SystemChecksController(ISystemCheckService service) => _service = service;

    [HttpPost("run")]
    [HasPermission(PermissionCodes.SystemChecksRun)]
    public async Task<ActionResult<ApiResponse<SystemCheckRunDto>>> Run(
            CancellationToken cancellationToken)
    {
        var result = await _service.RunAsync(cancellationToken);
        return Ok(ApiResponse<SystemCheckRunDto>.Success(result, "System checks completed successfully."));
    }
}

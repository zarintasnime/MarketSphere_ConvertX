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
[Route("api/lookups")]
public sealed class LookupsController : ControllerBase
{
    private readonly ILookupService _service;

    public LookupsController(ILookupService service) => _service = service;

    [HttpGet("{code}")]
    [HasPermission(PermissionCodes.LookupsView)]
    public async Task<ActionResult<ApiResponse<LookupGroupDto>>> Get(
            string code,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(code, cancellationToken);
        return Ok(ApiResponse<LookupGroupDto>.Success(result, "Lookup data retrieved successfully."));
    }
}

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
[Route("api/geography")]
public sealed class GeographyController : ControllerBase
{
    private readonly IGeographyService _service;

    public GeographyController(IGeographyService service) => _service = service;

    [HttpGet("regions")]
    [HasPermission(PermissionCodes.GeographyView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RegionDto>>>> GetRegions(
            [FromQuery] int? companyID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetRegionsAsync(companyID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RegionDto>>.Success(result, "Regions retrieved successfully."));
    }

    [HttpPost("regions")]
    [HasPermission(PermissionCodes.GeographyManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateRegion(
        [FromBody] CreateRegionRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateRegionAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Region created successfully."));
    }

    [HttpPut("regions/{regionID:int}")]
    [HasPermission(PermissionCodes.GeographyManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateRegion(
        int regionID,
        [FromBody] UpdateRegionRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateRegionAsync(regionID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Region updated successfully."));
    }

    [HttpGet("areas")]
    [HasPermission(PermissionCodes.GeographyView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<AreaDto>>>> GetAreas(
            [FromQuery] int? regionID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetAreasAsync(regionID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<AreaDto>>.Success(result, "Areas retrieved successfully."));
    }

    [HttpPost("areas")]
    [HasPermission(PermissionCodes.GeographyManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateArea(
        [FromBody] CreateAreaRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateAreaAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Area created successfully."));
    }

    [HttpPut("areas/{areaID:int}")]
    [HasPermission(PermissionCodes.GeographyManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateArea(
        int areaID,
        [FromBody] UpdateAreaRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateAreaAsync(areaID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Area updated successfully."));
    }

    [HttpGet("territories")]
    [HasPermission(PermissionCodes.GeographyView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<TerritoryDto>>>> GetTerritories(
            [FromQuery] int? areaID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetTerritoriesAsync(areaID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<TerritoryDto>>.Success(result, "Territories retrieved successfully."));
    }

    [HttpPost("territories")]
    [HasPermission(PermissionCodes.GeographyManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateTerritory(
        [FromBody] CreateTerritoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateTerritoryAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Territory created successfully."));
    }

    [HttpPut("territories/{territoryID:int}")]
    [HasPermission(PermissionCodes.GeographyManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTerritory(
        int territoryID,
        [FromBody] UpdateTerritoryRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateTerritoryAsync(territoryID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Territory updated successfully."));
    }
}

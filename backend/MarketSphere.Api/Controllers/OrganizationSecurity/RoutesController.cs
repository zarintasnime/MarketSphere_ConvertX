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
[Route("api/routes")]
public sealed class RoutesController : ControllerBase
{
    private readonly IRouteService _service;

    public RoutesController(IRouteService service) => _service = service;

    [HttpGet]
    [HasPermission(PermissionCodes.RoutesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RouteDto>>>> GetRoutes(
            [FromQuery] int? territoryID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetRoutesAsync(territoryID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RouteDto>>.Success(result, "Routes retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.RoutesManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateRoute(
        [FromBody] CreateRouteRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateRouteAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Route created successfully."));
    }

    [HttpPut("{routeID:int}")]
    [HasPermission(PermissionCodes.RoutesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateRoute(
        int routeID,
        [FromBody] UpdateRouteRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateRouteAsync(routeID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Route updated successfully."));
    }

    [HttpGet("{routeID:int}/outlets")]
    [HasPermission(PermissionCodes.RoutesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<RouteOutletDto>>>> GetRouteOutlets(
            int routeID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetRouteOutletsAsync(routeID, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<RouteOutletDto>>.Success(result, "Route outlets retrieved successfully."));
    }

    [HttpPost("outlets")]
    [HasPermission(PermissionCodes.RoutesManage)]
    public async Task<ActionResult<ApiResponse<int>>> AddRouteOutlet(
        [FromBody] CreateRouteOutletRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AddRouteOutletAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Route outlet added successfully."));
    }

    [HttpPost("employee-route-assignments")]
    [HasPermission(PermissionCodes.AssignmentsManage)]
    public async Task<ActionResult<ApiResponse<int>>> AssignEmployeeRoute(
        [FromBody] CreateEmployeeRouteAssignmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AssignEmployeeRouteAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Employee route assignment created successfully."));
    }

    [HttpPost("employee-territory-assignments")]
    [HasPermission(PermissionCodes.AssignmentsManage)]
    public async Task<ActionResult<ApiResponse<int>>> AssignEmployeeTerritory(
        [FromBody] CreateEmployeeTerritoryAssignmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.AssignEmployeeTerritoryAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Employee territory assignment created successfully."));
    }
}

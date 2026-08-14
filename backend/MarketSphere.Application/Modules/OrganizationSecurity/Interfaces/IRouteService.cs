using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IRouteService
{
    Task<IReadOnlyCollection<RouteDto>> GetRoutesAsync(
        int? territoryID = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateRouteAsync(
        CreateRouteRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateRouteAsync(
        int routeID,
        UpdateRouteRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RouteOutletDto>> GetRouteOutletsAsync(
        int routeID,
        CancellationToken cancellationToken = default);

    Task<int> AddRouteOutletAsync(
        CreateRouteOutletRequestDto request,
        CancellationToken cancellationToken = default);

    Task<int> AssignEmployeeRouteAsync(
        CreateEmployeeRouteAssignmentRequestDto request,
        CancellationToken cancellationToken = default);

    Task<int> AssignEmployeeTerritoryAsync(
        CreateEmployeeTerritoryAssignmentRequestDto request,
        CancellationToken cancellationToken = default);
}

using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;

public interface IGeographyService
{
    Task<IReadOnlyCollection<RegionDto>> GetRegionsAsync(
        int? companyID = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateRegionAsync(
        CreateRegionRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateRegionAsync(
        int regionID,
        UpdateRegionRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AreaDto>> GetAreasAsync(
        int? regionID = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateAreaAsync(
        CreateAreaRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAreaAsync(
        int areaID,
        UpdateAreaRequestDto request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TerritoryDto>> GetTerritoriesAsync(
        int? areaID = null,
        CancellationToken cancellationToken = default);

    Task<int> CreateTerritoryAsync(
        CreateTerritoryRequestDto request,
        CancellationToken cancellationToken = default);

    Task UpdateTerritoryAsync(
        int territoryID,
        UpdateTerritoryRequestDto request,
        CancellationToken cancellationToken = default);
}

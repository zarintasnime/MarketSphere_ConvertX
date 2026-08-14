using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.OrganizationSecurity.DTOs;
using MarketSphere.Application.Modules.OrganizationSecurity.Interfaces;
using MarketSphere.Domain.Entities.OrganizationSecurity;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.OrganizationSecurity.Services;

public sealed class GeographyService : IGeographyService
{
    private readonly IApplicationDbContext _db;

    public GeographyService(IApplicationDbContext db)
    {
        _db = db;
    }

    public Task<IReadOnlyCollection<RegionDto>> GetRegionsAsync(
        int? companyID = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Regions;

        if (companyID.HasValue)
            query = query.Where(x => x.CompanyID == companyID.Value);

        IReadOnlyCollection<RegionDto> items = query
            .OrderBy(x => x.RegionName)
            .Select(x => new RegionDto(
                x.RegionID,
                x.CompanyID,
                x.RegionCode,
                x.RegionName,
                x.IsActive))
            .ToArray();

        return Task.FromResult(items);
    }

    public async Task<int> CreateRegionAsync(
        CreateRegionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!_db.Companies.Any(
                x => x.CompanyID == request.CompanyID &&
                     x.IsActive))
        {
            throw new NotFoundException(
                $"Company with ID {request.CompanyID} was not found or is inactive.");
        }

        ValidationHelper.RequireNotBlank(
            request.RegionCode,
            nameof(request.RegionCode),
            50);

        ValidationHelper.RequireNotBlank(
            request.RegionName,
            nameof(request.RegionName),
            150);

        var code = request.RegionCode.NormalizeCode();

        if (_db.Regions.Any(
                x => x.CompanyID == request.CompanyID &&
                     x.RegionCode == code))
        {
            throw new ConflictException(
                $"Region code '{code}' already exists for this company.");
        }

        var region = new Region
        {
            CompanyID = request.CompanyID,
            RegionCode = code,
            RegionName = request.RegionName.Trim(),
            IsActive = true
        };

        await _db.AddAsync(region, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return region.RegionID;
    }

    public async Task UpdateRegionAsync(
        int regionID,
        UpdateRegionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var region = RequireRegion(regionID);

        ValidationHelper.RequireNotBlank(
            request.RegionName,
            nameof(request.RegionName),
            150);

        region.RegionName = request.RegionName.Trim();
        region.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<AreaDto>> GetAreasAsync(
        int? regionID = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Areas;

        if (regionID.HasValue)
            query = query.Where(x => x.RegionID == regionID.Value);

        IReadOnlyCollection<AreaDto> items = query
            .OrderBy(x => x.AreaName)
            .Select(x => new AreaDto(
                x.AreaID,
                x.RegionID,
                x.AreaCode,
                x.AreaName,
                x.IsActive))
            .ToArray();

        return Task.FromResult(items);
    }

    public async Task<int> CreateAreaAsync(
        CreateAreaRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var region = RequireRegion(request.RegionID);

        if (!region.IsActive)
        {
            throw new BusinessRuleException(
                "A new area cannot be created under an inactive region.");
        }

        ValidationHelper.RequireNotBlank(
            request.AreaCode,
            nameof(request.AreaCode),
            50);

        ValidationHelper.RequireNotBlank(
            request.AreaName,
            nameof(request.AreaName),
            150);

        var code = request.AreaCode.NormalizeCode();

        if (_db.Areas.Any(
                x => x.RegionID == request.RegionID &&
                     x.AreaCode == code))
        {
            throw new ConflictException(
                $"Area code '{code}' already exists for this region.");
        }

        var area = new Area
        {
            RegionID = request.RegionID,
            AreaCode = code,
            AreaName = request.AreaName.Trim(),
            IsActive = true
        };

        await _db.AddAsync(area, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return area.AreaID;
    }

    public async Task UpdateAreaAsync(
        int areaID,
        UpdateAreaRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var area = RequireArea(areaID);

        ValidationHelper.RequireNotBlank(
            request.AreaName,
            nameof(request.AreaName),
            150);

        area.AreaName = request.AreaName.Trim();
        area.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<TerritoryDto>> GetTerritoriesAsync(
        int? areaID = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Territories;

        if (areaID.HasValue)
            query = query.Where(x => x.AreaID == areaID.Value);

        IReadOnlyCollection<TerritoryDto> items = query
            .OrderBy(x => x.TerritoryName)
            .Select(x => new TerritoryDto(
                x.TerritoryID,
                x.AreaID,
                x.TerritoryCode,
                x.TerritoryName,
                x.IsActive))
            .ToArray();

        return Task.FromResult(items);
    }

    public async Task<int> CreateTerritoryAsync(
        CreateTerritoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var area = RequireArea(request.AreaID);

        if (!area.IsActive)
        {
            throw new BusinessRuleException(
                "A new territory cannot be created under an inactive area.");
        }

        ValidationHelper.RequireNotBlank(
            request.TerritoryCode,
            nameof(request.TerritoryCode),
            50);

        ValidationHelper.RequireNotBlank(
            request.TerritoryName,
            nameof(request.TerritoryName),
            150);

        var code = request.TerritoryCode.NormalizeCode();

        if (_db.Territories.Any(
                x => x.AreaID == request.AreaID &&
                     x.TerritoryCode == code))
        {
            throw new ConflictException(
                $"Territory code '{code}' already exists for this area.");
        }

        var territory = new Territory
        {
            AreaID = request.AreaID,
            TerritoryCode = code,
            TerritoryName = request.TerritoryName.Trim(),
            IsActive = true
        };

        await _db.AddAsync(territory, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return territory.TerritoryID;
    }

    public async Task UpdateTerritoryAsync(
        int territoryID,
        UpdateTerritoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var territory = RequireTerritory(territoryID);

        ValidationHelper.RequireNotBlank(
            request.TerritoryName,
            nameof(request.TerritoryName),
            150);

        territory.TerritoryName =
            request.TerritoryName.Trim();
        territory.IsActive = request.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
    }

    private Region RequireRegion(int regionID) =>
        _db.Regions.FirstOrDefault(x => x.RegionID == regionID)
        ?? throw new NotFoundException(
            $"Region with ID {regionID} was not found.");

    private Area RequireArea(int areaID) =>
        _db.Areas.FirstOrDefault(x => x.AreaID == areaID)
        ?? throw new NotFoundException(
            $"Area with ID {areaID} was not found.");

    private Territory RequireTerritory(int territoryID) =>
        _db.Territories.FirstOrDefault(
            x => x.TerritoryID == territoryID)
        ?? throw new NotFoundException(
            $"Territory with ID {territoryID} was not found.");
}

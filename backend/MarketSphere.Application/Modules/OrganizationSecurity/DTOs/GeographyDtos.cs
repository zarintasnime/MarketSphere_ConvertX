using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed record RegionDto(
    int RegionID,
    int CompanyID,
    string RegionCode,
    string RegionName,
    bool IsActive);

public sealed record AreaDto(
    int AreaID,
    int RegionID,
    string AreaCode,
    string AreaName,
    bool IsActive);

public sealed record TerritoryDto(
    int TerritoryID,
    int AreaID,
    string TerritoryCode,
    string TerritoryName,
    bool IsActive);

public sealed class CreateRegionRequestDto
{
    public int CompanyID { get; init; }

    [Required, MaxLength(50)]
    public string RegionCode { get; init; } = string.Empty;

    [Required, MaxLength(150)]
    public string RegionName { get; init; } = string.Empty;
}

public sealed class UpdateRegionRequestDto
{
    [Required, MaxLength(150)]
    public string RegionName { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}

public sealed class CreateAreaRequestDto
{
    public int RegionID { get; init; }

    [Required, MaxLength(50)]
    public string AreaCode { get; init; } = string.Empty;

    [Required, MaxLength(150)]
    public string AreaName { get; init; } = string.Empty;
}

public sealed class UpdateAreaRequestDto
{
    [Required, MaxLength(150)]
    public string AreaName { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}

public sealed class CreateTerritoryRequestDto
{
    public int AreaID { get; init; }

    [Required, MaxLength(50)]
    public string TerritoryCode { get; init; } = string.Empty;

    [Required, MaxLength(150)]
    public string TerritoryName { get; init; } = string.Empty;
}

public sealed class UpdateTerritoryRequestDto
{
    [Required, MaxLength(150)]
    public string TerritoryName { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}

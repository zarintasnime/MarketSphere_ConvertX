using MarketSphere.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed record RouteDto(
    int RouteID,
    int TerritoryID,
    string RouteCode,
    string RouteName,
    DayOfWeek? DayOfWeek,
    VisitFrequency VisitFrequency,
    bool IsActive);

public sealed class CreateRouteRequestDto
{
    public int TerritoryID { get; init; }

    [Required, MaxLength(50)]
    public string RouteCode { get; init; } = string.Empty;

    [Required, MaxLength(150)]
    public string RouteName { get; init; } = string.Empty;

    public DayOfWeek? DayOfWeek { get; init; }
    public VisitFrequency VisitFrequency { get; init; }
}

public sealed class UpdateRouteRequestDto
{
    [Required, MaxLength(150)]
    public string RouteName { get; init; } = string.Empty;

    public DayOfWeek? DayOfWeek { get; init; }
    public VisitFrequency VisitFrequency { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed record RouteOutletDto(
    int RouteOutletID,
    int RouteID,
    int ClientID,
    int SequenceNo,
    VisitFrequency VisitFrequency,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);

public sealed class CreateRouteOutletRequestDto
{
    public int RouteID { get; init; }
    public int ClientID { get; init; }

    [Range(1, int.MaxValue)]
    public int SequenceNo { get; init; }

    public VisitFrequency VisitFrequency { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
}

public sealed record EmployeeRouteAssignmentDto(
    int EmployeeRouteAssignmentID,
    int EmployeeID,
    int RouteID,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    DayOfWeek? DayOfWeek,
    bool IsPrimary,
    AssignmentStatus Status);

public sealed class CreateEmployeeRouteAssignmentRequestDto
{
    public int EmployeeID { get; init; }
    public int RouteID { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public DayOfWeek? DayOfWeek { get; init; }
    public bool IsPrimary { get; init; }
}

public sealed record EmployeeTerritoryAssignmentDto(
    int EmployeeTerritoryAssignmentID,
    int EmployeeID,
    GeographyScopeType ScopeType,
    int? RegionID,
    int? AreaID,
    int? TerritoryID,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsPrimary);

public sealed class CreateEmployeeTerritoryAssignmentRequestDto
{
    public int EmployeeID { get; init; }
    public GeographyScopeType ScopeType { get; init; }
    public int? RegionID { get; init; }
    public int? AreaID { get; init; }
    public int? TerritoryID { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public bool IsPrimary { get; init; }
}

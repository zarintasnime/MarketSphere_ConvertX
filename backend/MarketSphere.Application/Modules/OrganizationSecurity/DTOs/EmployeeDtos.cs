using MarketSphere.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed class CreateEmployeeRequestDto
{
    [Required, MaxLength(50)]
    public string EmployeeCode { get; init; } = string.Empty;

    public int? UserID { get; init; }
    public int DesignationID { get; init; }
    public int? ManagerEmployeeID { get; init; }
    public int BranchID { get; init; }
    public int? RegionID { get; init; }
    public int? AreaID { get; init; }
    public int? TerritoryID { get; init; }
    public DateOnly JoiningDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public EmployeeStatus Status { get; init; } = EmployeeStatus.Active;

    [MaxLength(30)]
    public string? Phone { get; init; }

    [EmailAddress, MaxLength(256)]
    public string? Email { get; init; }
}

public sealed class UpdateEmployeeRequestDto
{
    public int? UserID { get; init; }
    public int DesignationID { get; init; }
    public int? ManagerEmployeeID { get; init; }
    public int BranchID { get; init; }
    public int? RegionID { get; init; }
    public int? AreaID { get; init; }
    public int? TerritoryID { get; init; }
    public DateOnly JoiningDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public EmployeeStatus Status { get; init; }

    [MaxLength(30)]
    public string? Phone { get; init; }

    [EmailAddress, MaxLength(256)]
    public string? Email { get; init; }
}

public sealed record EmployeeListItemDto(
    int EmployeeID,
    string EmployeeCode,
    int? UserID,
    string? UserFullName,
    int DesignationID,
    string DesignationName,
    int BranchID,
    string BranchName,
    EmployeeStatus Status);

public sealed record EmployeeDetailsDto(
    int EmployeeID,
    string EmployeeCode,
    int? UserID,
    int DesignationID,
    int? ManagerEmployeeID,
    int BranchID,
    int? RegionID,
    int? AreaID,
    int? TerritoryID,
    DateOnly JoiningDate,
    DateOnly? EndDate,
    EmployeeStatus Status,
    string? Phone,
    string? Email);

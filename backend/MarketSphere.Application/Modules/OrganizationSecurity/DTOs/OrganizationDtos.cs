using MarketSphere.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MarketSphere.Application.Modules.OrganizationSecurity.DTOs;

public sealed record CompanyDto(
    int CompanyID,
    string CompanyCode,
    string CompanyName,
    string? TradeLicenseNo,
    string? Phone,
    string? Email,
    string? Address,
    bool IsActive);

public sealed class UpdateCompanyRequestDto
{
    [Required, MaxLength(150)]
    public string CompanyName { get; init; } = string.Empty;

    [MaxLength(100)]
    public string? TradeLicenseNo { get; init; }

    [MaxLength(30)]
    public string? Phone { get; init; }

    [EmailAddress, MaxLength(256)]
    public string? Email { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }

    public bool IsActive { get; init; } = true;
}

public sealed record BranchDto(
    int BranchID,
    int CompanyID,
    string BranchCode,
    string BranchName,
    BranchType BranchType,
    string? Address,
    string? Phone,
    bool IsActive);

public sealed class CreateBranchRequestDto
{
    public int CompanyID { get; init; }

    [Required, MaxLength(50)]
    public string BranchCode { get; init; } = string.Empty;

    [Required, MaxLength(150)]
    public string BranchName { get; init; } = string.Empty;

    public BranchType BranchType { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }

    [MaxLength(30)]
    public string? Phone { get; init; }
}

public sealed class UpdateBranchRequestDto
{
    [Required, MaxLength(150)]
    public string BranchName { get; init; } = string.Empty;

    public BranchType BranchType { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }

    [MaxLength(30)]
    public string? Phone { get; init; }

    public bool IsActive { get; init; } = true;
}

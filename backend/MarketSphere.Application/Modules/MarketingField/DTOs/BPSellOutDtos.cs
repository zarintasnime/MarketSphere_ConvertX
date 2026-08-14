using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.MarketingField.DTOs;

public sealed record BPSellOutListDto(int BPSellOutID, int EmployeeID, int ClientID, int? VisitID, int? CampaignID, DateOnly SellOutDate, decimal TotalQuantity, decimal TotalValue, VerificationStatus VerificationStatus, int? VerifiedByEmployeeID, DateTime? VerifiedAt);
public sealed record BPSellOutItemDto(int BPSellOutItemID, int SKUID, decimal QuantitySold, decimal? UnitSellingPrice, decimal? LineValue);
public sealed record BPSellOutDetailsDto(int BPSellOutID, int EmployeeID, int ClientID, int? VisitID, int? CampaignID, DateOnly SellOutDate, decimal TotalQuantity, decimal TotalValue, decimal? GPSLat, decimal? GPSLng, VerificationStatus VerificationStatus, int? VerifiedByEmployeeID, DateTime? VerifiedAt, IReadOnlyCollection<BPSellOutItemDto> Items);
public sealed class SaveBPSellOutItemRequestDto { public int SKUID { get; init; } public decimal QuantitySold { get; init; } public decimal? UnitSellingPrice { get; init; } }
public sealed class SaveBPSellOutRequestDto { public int EmployeeID { get; init; } public int ClientID { get; init; } public int? VisitID { get; init; } public int? CampaignID { get; init; } public DateOnly SellOutDate { get; init; } public decimal? GPSLat { get; init; } public decimal? GPSLng { get; init; } public IReadOnlyCollection<SaveBPSellOutItemRequestDto> Items { get; init; } = Array.Empty<SaveBPSellOutItemRequestDto>(); }
public sealed class VerifyBPSellOutRequestDto { public int VerifiedByEmployeeID { get; init; } public VerificationStatus VerificationStatus { get; init; } public string? Note { get; init; } }

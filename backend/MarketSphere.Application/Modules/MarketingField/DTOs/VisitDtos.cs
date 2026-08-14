using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.MarketingField.DTOs;

public sealed record VisitListDto(int VisitID, int EmployeeID, int ClientID, int? RouteID, int? CampaignID, VisitType VisitType, DateTime CheckInAt, DateTime? CheckOutAt, VisitStatus Status, bool IsSuspiciousLocation);
public sealed record VisitDetailsDto(int VisitID, int EmployeeID, int ClientID, int? RouteID, int? CampaignID, VisitType VisitType, DateTime CheckInAt, DateTime? CheckOutAt, decimal CheckInGPSLat, decimal CheckInGPSLng, decimal? CheckOutGPSLat, decimal? CheckOutGPSLng, decimal? AccuracyMeters, bool IsSuspiciousLocation, string? Note, VisitStatus Status);
public sealed class CheckInVisitRequestDto { public int EmployeeID { get; init; } public int ClientID { get; init; } public int? RouteID { get; init; } public int? CampaignID { get; init; } public VisitType VisitType { get; init; } public DateTime? CheckInAt { get; init; } public decimal CheckInGPSLat { get; init; } public decimal CheckInGPSLng { get; init; } public decimal? AccuracyMeters { get; init; } public string? Note { get; init; } }
public sealed class CheckOutVisitRequestDto { public DateTime? CheckOutAt { get; init; } public decimal CheckOutGPSLat { get; init; } public decimal CheckOutGPSLng { get; init; } public string? Note { get; init; } }
public sealed class CancelVisitRequestDto { public string Reason { get; init; } = string.Empty; }

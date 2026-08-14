using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.MarketingField.DTOs;

public sealed record FieldActiveVisitDto(
    int VisitID,
    int EmployeeID,
    int ClientID,
    string ClientCode,
    string ClientName,
    int? RouteID,
    string? RouteName,
    int? CampaignID,
    VisitType VisitType,
    DateTime CheckInAt,
    decimal CheckInGPSLat,
    decimal CheckInGPSLng,
    decimal? AccuracyMeters,
    bool IsSuspiciousLocation,
    string? Note);

public sealed record FieldAssignedClientDto(
    int ClientID,
    string ClientCode,
    string ClientName,
    ClientType ClientType,
    SalesChannel Channel,
    string? Phone,
    string Address,
    decimal? GPSLat,
    decimal? GPSLng,
    int? RegionID,
    int? AreaID,
    int? TerritoryID,
    int? RouteID,
    string? RouteCode,
    string? RouteName,
    int? SequenceNo);

public sealed record FieldVisitListDto(
    int VisitID,
    int ClientID,
    string ClientCode,
    string ClientName,
    int? RouteID,
    string? RouteName,
    int? CampaignID,
    VisitType VisitType,
    DateTime CheckInAt,
    DateTime? CheckOutAt,
    VisitStatus Status,
    bool IsSuspiciousLocation);

public sealed record FieldWorkspaceSummaryDto(
    int EmployeeID,
    string EmployeeCode,
    string EmployeeName,
    string DesignationName,
    int BranchID,
    string BranchName,
    int AssignedClientCount,
    int TodayVisitCount,
    int CompletedVisitCount,
    int UnreadNotificationCount,
    FieldActiveVisitDto? ActiveVisit);

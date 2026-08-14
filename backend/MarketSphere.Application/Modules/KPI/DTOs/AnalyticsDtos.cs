using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.KPI.DTOs;

public sealed record DashboardKpiDto(
    string Code,
    string Label,
    decimal Value,
    string? Unit,
    decimal? ChangePercent);

public sealed record FunnelPointDto(
    string Stage,
    int Count,
    decimal Value);

public sealed record SeriesPointDto(
    DateTime Period,
    decimal Value,
    string? Series);

public sealed record ExecutiveDashboardDto(
    IReadOnlyCollection<DashboardKpiDto> Kpis,
    IReadOnlyCollection<FunnelPointDto> LeadToOrderFunnel,
    IReadOnlyCollection<SeriesPointDto> SalesTrend,
    int PendingApprovals,
    int OverdueTasks,
    int NearExpiryBatches);

public sealed record AnalyticsFilterDto(
    DateTime From,
    DateTime To,
    int? BranchID,
    int? RegionID,
    int? EmployeeID,
    int? CampaignID);

public sealed record CampaignRoiDto(
    int CampaignID,
    string CampaignCode,
    string CampaignTitle,
    decimal Budget,
    decimal Expense,
    decimal AttributedValue,
    decimal DeliveredValue,
    decimal RoiPercent);

public sealed record ChannelSalesDto(
    SalesChannel Channel,
    int OrderCount,
    decimal GrossAmount,
    decimal NetAmount,
    decimal DeliveredValue);

public sealed record SellInSellOutPointDto(
    DateTime Period,
    decimal SellInQuantity,
    decimal SellInValue,
    decimal SellOutQuantity,
    decimal SellOutValue);

public sealed record InventoryHealthItemDto(
    int WarehouseID,
    string WarehouseName,
    int SKUID,
    string SKUCode,
    string SKUName,
    int? BatchID,
    string? BatchNo,
    DateTime? ExpiryDate,
    BatchStatus? BatchStatus,
    decimal OnHandQuantity,
    decimal ReservedQuantity,
    decimal QuarantineQuantity,
    decimal DamagedQuantity,
    decimal AvailableQuantity,
    bool IsLowStock,
    bool IsNearExpiry);

public sealed record InventoryHealthDto(
    decimal OnHandQuantity,
    decimal AvailableQuantity,
    decimal ReservedQuantity,
    decimal QuarantineQuantity,
    decimal DamagedQuantity,
    int NearExpiryBatchCount,
    int ExpiredBatchCount,
    int LowStockSkuCount,
    decimal LowStockThreshold,
    IReadOnlyCollection<InventoryHealthItemDto> Items);

public sealed record DeliveryReturnPointDto(
    DateTime Period,
    int PlannedCount,
    int DeliveredCount,
    int PartialCount,
    int FailedCount,
    int RescheduledCount,
    int ReturnRequestCount,
    decimal ReturnedQuantity);

public sealed record EmployeeKpiDto(
    int EmployeeID,
    string EmployeeCode,
    string EmployeeName,
    decimal TargetValue,
    decimal ActualValue,
    decimal AchievementPercent,
    decimal RewardAmount);

public sealed record Client360HeaderDto(
    int ClientID,
    string ClientCode,
    string ClientName,
    ClientType ClientType,
    SalesChannel Channel,
    string? Phone,
    string? Email,
    string Address,
    ClientLifecycleStatus LifecycleStatus,
    ClientRiskStatus RiskStatus,
    decimal CreditLimit,
    decimal CurrentDue,
    bool IsCreditBlocked,
    int OrderCount,
    decimal OrderValue,
    decimal PaidAmount,
    int OpenComplaintCount);

public sealed record Client360OrderDto(
    int OrderID,
    string OrderNo,
    DateTime OrderDate,
    SalesChannel Channel,
    OrderStatus Status,
    decimal NetAmount);

public sealed record Client360PaymentDto(
    int PaymentID,
    string PaymentNo,
    DateTime PaymentDate,
    PaymentMethod PaymentMethod,
    CustomerPaymentStatus Status,
    decimal Amount);

public sealed record Client360ComplaintDto(
    int ComplaintID,
    string ComplaintNo,
    DateTime OpenedAt,
    ComplaintPriority Priority,
    ComplaintStatus Status,
    string Subject);

public sealed record Client360TimelineItemDto(
    DateTime OccurredAt,
    string Type,
    string Title,
    string? Status,
    decimal? Amount,
    int? ReferenceID);

public sealed record Client360Dto(
    Client360HeaderDto Header,
    IReadOnlyCollection<Client360OrderDto> RecentOrders,
    IReadOnlyCollection<Client360PaymentDto> RecentPayments,
    IReadOnlyCollection<Client360ComplaintDto> RecentComplaints,
    IReadOnlyCollection<Client360TimelineItemDto> Timeline);

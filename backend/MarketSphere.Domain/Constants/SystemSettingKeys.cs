namespace MarketSphere.Domain.Constants;

public static class SystemSettingKeys
{
    public const string PasswordMinimumLength = "Security.PasswordMinimumLength";
    public const string LoginLockoutThreshold = "Security.LoginLockoutThreshold";
    public const string LoginLockoutMinutes = "Security.LoginLockoutMinutes";
    public const string AccessTokenMinutes = "Security.AccessTokenMinutes";
    public const string RefreshTokenDays = "Security.RefreshTokenDays";
    public const string DefaultPurchaseOrderTolerancePercent = "Procurement.DefaultPurchaseOrderTolerancePercent";
    public const string NearExpiryAlertDays = "Inventory.NearExpiryAlertDays";
    public const string StockTransferVariancePercent = "Inventory.StockTransferVariancePercent";
    public const string AllowNegativeStock = "Inventory.AllowNegativeStock";

    public const string InactiveClientDays = "CRM.InactiveClientDays";
    public const string QuotationExpiryAlertDays = "CRM.QuotationExpiryAlertDays";
    public const string ComplaintDefaultSlaHours = "CRM.ComplaintDefaultSlaHours";
    public const string OfferConflictMode = "Marketing.OfferConflictMode";
    public const string OfflineRetryLimit = "Offline.RetryLimit";
    public const string IdempotencyRetentionHours = "API.IdempotencyRetentionHours";
    public const string NotificationRetentionDays = "Notifications.RetentionDays";
}

namespace MarketSphere.Domain.Enums;

public enum AccountTokenUseResult
{
    Valid = 1,
    Expired = 2,
    Used = 3,
    Invalid = 4
}

public enum NotificationType
{
    Information = 1,
    Warning = 2,
    ActionRequired = 3,
    Approval = 4,
    Expiry = 5,
    Sla = 6,
    System = 7
}

public enum NotificationPriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Critical = 4
}

public enum ApprovalType
{
    Discount = 1,
    CreditOverride = 2,
    Order = 3,
    PurchaseRequisition = 4,
    PurchaseOrder = 5,
    StockTransfer = 6,
    StockAdjustment = 7,
    Return = 8,
    SupplierReturn = 9,
    Reward = 10,
    Other = 99
}

public enum ApprovalMode
{
    AnyOne = 1,
    All = 2,
    MinimumCount = 3
}

public enum ApprovalAssigneeType
{
    Role = 1,
    Designation = 2,
    User = 3,
    Employee = 4
}

public enum ApprovalRequestStatus
{
    Pending = 1,
    InProgress = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5
}

public enum ApprovalActionType
{
    Submitted = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4,
    Delegated = 5,
    Commented = 6
}

public enum FileVerificationStatus
{
    Pending = 1,
    Verified = 2,
    Rejected = 3
}

public enum SettingDataType
{
    String = 1,
    Integer = 2,
    Decimal = 3,
    Boolean = 4,
    DateTime = 5,
    Json = 6
}

public enum SettingScopeType
{
    Global = 1,
    Company = 2,
    Branch = 3,
    User = 4
}

public enum OfflineOperationType
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Submit = 4
}

public enum OfflineSyncStatus
{
    Pending = 1,
    Processing = 2,
    Synced = 3,
    Failed = 4,
    Conflict = 5
}

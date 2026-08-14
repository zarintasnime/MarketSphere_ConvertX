namespace MarketSphere.Domain.Enums;

public enum ModernTradePurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Verified = 3,
    Rejected = 4,
    Converted = 5,
    Cancelled = 6
}

public enum ModernTradeVerificationStatus
{
    Pending = 1,
    Incomplete = 2,
    MappingRequired = 3,
    Verified = 4,
    Rejected = 5
}

public enum ModernTradeCompletenessStatus
{
    Incomplete = 1,
    Complete = 2
}

public enum ItemMappingStatus
{
    Unmapped = 1,
    Mapped = 2,
    Rejected = 3
}

public enum OrderSource
{
    Regular = 1,
    Quotation = 2,
    ModernTradePurchaseOrder = 3,
    Campaign = 4
}

public enum OrderStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Approved = 4,
    StockAllocated = 5,
    Invoiced = 6,
    ReadyForDispatch = 7,
    PartiallyDelivered = 8,
    Delivered = 9,
    Returned = 10,
    Closed = 11,
    Rejected = 12,
    Cancelled = 13
}

public enum CreditCheckStatus
{
    NotRequired = 1,
    Pending = 2,
    Passed = 3,
    Failed = 4,
    OverrideRequired = 5
}

public enum AppliedBenefitType
{
    PercentageDiscount = 1,
    FixedDiscount = 2,
    FreeItem = 3,
    Bundle = 4,
    Cashback = 5,
    Other = 6
}

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    PartiallyPaid = 3,
    Paid = 4,
    PartiallyCredited = 5,
    Credited = 6,
    Cancelled = 7
}

public enum PickListStatus
{
    Draft = 1,
    Released = 2,
    Picking = 3,
    PartiallyPicked = 4,
    Picked = 5,
    Verified = 6,
    Cancelled = 7
}

public enum DeliveryStatus
{
    Pending = 1,
    ReadyForDispatch = 2,
    Dispatched = 3,
    PartiallyDelivered = 4,
    Delivered = 5,
    Failed = 6,
    Rescheduled = 7,
    Cancelled = 8
}

public enum ReturnRequestStatus
{
    Requested = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Received = 5,
    Inspected = 6,
    Resolved = 7,
    Closed = 8
}

public enum ReturnResolutionType
{
    Restock = 1,
    Quarantine = 2,
    Damage = 3,
    Replacement = 4,
    Credit = 5,
    SupplierClaim = 6,
    Mixed = 7
}

public enum ReturnConditionStatus
{
    Unopened = 1,
    Saleable = 2,
    Damaged = 3,
    Expired = 4,
    Defective = 5,
    WrongItem = 6,
    Other = 7
}

public enum ReturnDisposition
{
    Pending = 1,
    Restock = 2,
    Quarantine = 3,
    Damage = 4,
    Replace = 5,
    Credit = 6,
    SupplierReturn = 7
}

public enum CreditNoteStatus
{
    Draft = 1,
    Posted = 2,
    Cancelled = 3
}

public enum CustomerPaymentStatus
{
    Pending = 1,
    Confirmed = 2,
    Rejected = 3,
    Reversed = 4
}

public enum PaymentAllocationType
{
    Allocation = 1,
    Reversal = 2
}

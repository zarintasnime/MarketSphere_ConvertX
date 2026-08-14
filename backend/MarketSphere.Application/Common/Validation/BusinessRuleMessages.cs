namespace MarketSphere.Application.Common.Validation;

public static class BusinessRuleMessages
{
    public const string DuplicateCode = "The supplied code already exists.";
    public const string DuplicateEmail =
    "The supplied email address is already in use.";
    public const string InvalidStatusTransition = "The requested status transition is not allowed.";
    public const string EntityNotFound = "The requested record was not found.";
    public const string ActiveRecordRequired = "The referenced record must be active.";
    public const string GeographyMismatch = "The selected geography hierarchy is inconsistent.";
    public const string RelatedEntityRequired = "At least one related CRM entity is required.";
    public const string ParticipantIdentityRequired = "At least one participant identity is required.";
    public const string LostReasonRequired = "A lost reason is required.";
    public const string ResolutionNoteRequired = "A resolution note is required.";
    public const string QuotationImmutable = "A submitted quotation version cannot be edited.";
    public const string QuotationVersionConflict = "The quotation version already exists.";
    public const string DuplicateReviewSelfMatch = "A duplicate review source and match cannot be the same record.";
    public const string CreditCannotBeNegative = "Credit values cannot be negative.";
    public const string CurrentDueCannotBeNegative = "Current due cannot be negative.";

    public const string CampaignDateRangeInvalid = "Campaign end date must be on or after the start date.";
    public const string CampaignLocked = "The campaign cannot be edited in its current status.";
    public const string CampaignTargetInvalid = "The selected target fields do not match the target type.";
    public const string SamplingBalanceInvalid = "Issued quantity must equal consumed, returned and damaged quantities.";
    public const string VisitAlreadyCompleted = "The visit is already completed.";
    public const string SellOutLocked = "A verified or rejected sell-out cannot be edited.";
    public const string SellOutTotalsInvalid = "Sell-out header totals must match the item totals.";

    public const string ProductCategoryCircularReference = "A product category cannot be its own parent or descendant.";
    public const string ProductExpiryRequiresBatch = "A product that requires expiry tracking must also require batch tracking.";
    public const string PriceListPeriodConflict = "An active price list overlaps another active price list for the same scope.";
    public const string DiscountRulePeriodConflict = "An active discount rule overlaps another rule with the same specificity and scope.";
    public const string PriceListIsNotApplicable = "No applicable active price list item was found for the requested SKU and scope.";
    public const string DocumentLocked = "The document cannot be edited in its current status.";
    public const string QuantityMustBePositive = "Quantity must be greater than zero.";
    public const string InvalidDocumentTotal = "The document totals are inconsistent.";
    public const string PurchaseOrderToleranceExceeded = "Received quantity exceeds the allowed purchase-order tolerance.";
    public const string BatchRequired = "Batch is required for this product.";
    public const string ExpiryDateRequired = "Expiry date is required for this product.";
    public const string InsufficientStock = "Available stock is insufficient for this operation.";
    public const string NegativeStockBlocked = "The operation would create a negative stock balance.";
    public const string WarehouseMustDiffer = "Source and destination warehouses must be different.";
    public const string StockDocumentAlreadyPosted = "The stock document has already been posted.";
    public const string ModernTradePurchaseOrderNotVerified = "The modern-trade purchase order must be complete, fully mapped and verified.";
    public const string SourceAlreadyConverted = "The source document has already been converted to an order.";
    public const string OrderCreditCheckFailed = "The order failed the client credit check.";
    public const string OrderAlreadyReserved = "Stock has already been reserved for this order.";
    public const string InsufficientFefoStock = "Valid FEFO stock is insufficient for the approved quantity.";
    public const string OfferParentInvalid = "An applied offer must belong to exactly one supported document or line parent.";
    public const string InvoiceQuantityExceeded = "Invoice quantity exceeds the approved uninvoiced quantity.";
    public const string PickListVerificationRequired = "A pick list must be verified before delivery dispatch.";
    public const string PickQuantityInvalid = "Picked and short quantities cannot exceed the requested quantity.";
    public const string DeliveryQuantityInvalid = "Delivered and rejected quantities cannot exceed the dispatched quantity.";
    public const string ReturnQuantityInvalid = "Return quantity exceeds the remaining delivered quantity.";
    public const string ReturnDispositionInvalid = "Return disposition quantities must equal the received quantity.";
    public const string AllocationLimitExceeded = "The allocation exceeds the available payment amount or invoice due amount.";
    public const string AllocationAlreadyReversed = "The payment allocation has already been reversed.";
}

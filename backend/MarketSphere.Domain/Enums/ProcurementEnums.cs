namespace MarketSphere.Domain.Enums;

public enum SupplierStatus { Active = 1, Suspended = 2, Inactive = 3 }
public enum PurchaseRequisitionStatus { Draft = 1, Submitted = 2, Approved = 3, Rejected = 4, Closed = 5, Cancelled = 6 }
public enum PurchaseOrderStatus { Draft = 1, Submitted = 2, Approved = 3, PartiallyReceived = 4, Received = 5, Closed = 6, Cancelled = 7 }
public enum GoodsReceiptStatus { Draft = 1, QualityCheck = 2, Approved = 3, Rejected = 4, Posted = 5 }
public enum QualityCheckStatus { Pending = 1, Passed = 2, PartiallyAccepted = 3, Failed = 4 }
public enum PurchaseInvoiceStatus { Draft = 1, Confirmed = 2, Cancelled = 3 }
public enum SupplierInvoicePaymentStatus { Unpaid = 1, PartiallyPaid = 2, Paid = 3 }
public enum SupplierPaymentStatus { Pending = 1, Confirmed = 2, Rejected = 3, Reversed = 4 }
public enum SupplierReturnStatus { Draft = 1, Submitted = 2, Approved = 3, Posted = 4, Cancelled = 5 }
public enum PaymentMethod { Cash = 1, BankTransfer = 2, Cheque = 3, MobileFinancialService = 4, Other = 5 }

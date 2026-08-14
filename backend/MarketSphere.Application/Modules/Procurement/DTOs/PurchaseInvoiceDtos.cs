using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.Procurement.DTOs;

public sealed record SavePurchaseInvoiceRequestDto(string PurchaseInvoiceNo, int SupplierID, int? PurchaseOrderID, int? GoodsReceiptID, DateTime InvoiceDate, DateTime? DueDate, decimal GrossAmount, decimal DiscountAmount, decimal TaxAmount);
public sealed record PurchaseInvoiceDto(int PurchaseInvoiceID, string PurchaseInvoiceNo, int SupplierID, string SupplierName, int? PurchaseOrderID, int? GoodsReceiptID, DateTime InvoiceDate, DateTime? DueDate, decimal TotalAmount, decimal PaidAmount, decimal DueAmount, SupplierInvoicePaymentStatus PaymentStatus, PurchaseInvoiceStatus Status);
public sealed record CreateSupplierPaymentRequestDto(int PurchaseInvoiceID, string PaymentNo, DateTime PaymentDate, PaymentMethod PaymentMethod, decimal Amount, string? ReferenceNo);
public sealed record SupplierPaymentDto(int SupplierPaymentID, string PaymentNo, int PurchaseInvoiceID, DateTime PaymentDate, PaymentMethod PaymentMethod, decimal Amount, string? ReferenceNo, SupplierPaymentStatus Status);
public sealed record ChangeSupplierPaymentStatusRequestDto(SupplierPaymentStatus Status);

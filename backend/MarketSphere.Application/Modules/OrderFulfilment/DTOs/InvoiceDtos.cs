using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.OrderFulfilment.DTOs;

public sealed record InvoiceItemDto(int InvoiceItemID, int OrderItemID, int SKUID, string SKUCode, decimal Quantity, decimal UnitPrice, decimal DiscountAmount, decimal TaxAmount, decimal LineTotal);
public sealed record InvoiceListDto(int InvoiceID, string InvoiceNo, int OrderID, int ClientID, DateTime InvoiceDate, DateTime? DueDate, decimal TotalAmount, decimal PaidAmount, decimal DueAmount, InvoiceStatus Status);
public sealed record InvoiceDetailsDto(int InvoiceID, string InvoiceNo, int OrderID, int ClientID, DateTime InvoiceDate, DateTime? DueDate, decimal GrossAmount, decimal DiscountAmount, decimal TaxAmount, decimal TotalAmount, decimal PaidAmount, decimal DueAmount, InvoiceStatus Status, IReadOnlyCollection<InvoiceItemDto> Items);
public sealed class CreateInvoiceRequestDto { public string InvoiceNo { get; init; } = string.Empty; public int OrderID { get; init; } public DateTime InvoiceDate { get; init; } public DateTime? DueDate { get; init; } public IReadOnlyCollection<CreateInvoiceItemRequestDto> Items { get; init; } = Array.Empty<CreateInvoiceItemRequestDto>(); }
public sealed class CreateInvoiceItemRequestDto { public int OrderItemID { get; init; } public decimal Quantity { get; init; } }
public sealed class ChangeInvoiceStatusRequestDto { public InvoiceStatus Status { get; init; } }

using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.CRM.DTOs;

public sealed record QuotationItemDto(int QuotationItemID, int SKUID, decimal Quantity, decimal UnitPrice, decimal DiscountPercent, decimal DiscountAmount, decimal TaxAmount, decimal LineTotal, string? Note);
public sealed record QuotationListDto(int QuotationID, string QuotationNo, int VersionNo, int ClientID, int? OpportunityID, DateOnly ValidFrom, DateOnly ValidUntil, QuotationStatus Status, decimal NetAmount);
public sealed record QuotationDetailsDto(int QuotationID, int? RootQuotationID, int VersionNo, string QuotationNo, int? OpportunityID, int ClientID, int? CampaignID, int? PriceListID, DateOnly ValidFrom, DateOnly ValidUntil, QuotationStatus Status, decimal GrossAmount, decimal DiscountAmount, decimal TaxAmount, decimal NetAmount, string? Terms, DateTime? AcceptedAt, IReadOnlyCollection<QuotationItemDto> Items);
public sealed class SaveQuotationRequestDto { public string QuotationNo { get; init; } = string.Empty; public int? OpportunityID { get; init; } public int ClientID { get; init; } public int? CampaignID { get; init; } public int? PriceListID { get; init; } public DateOnly ValidFrom { get; init; } public DateOnly ValidUntil { get; init; } public string? Terms { get; init; } public IReadOnlyCollection<SaveQuotationItemRequestDto> Items { get; init; } = Array.Empty<SaveQuotationItemRequestDto>(); }
public sealed class SaveQuotationItemRequestDto { public int SKUID { get; init; } public decimal Quantity { get; init; } public decimal UnitPrice { get; init; } public decimal DiscountPercent { get; init; } public decimal TaxAmount { get; init; } public string? Note { get; init; } }
public sealed class ChangeQuotationStatusRequestDto { public QuotationStatus Status { get; init; } }

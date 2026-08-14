using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.ProductPricing.DTOs;

public sealed record PriceListItemDto(
    int PriceListItemID,
    int SKUID,
    string SKUCode,
    string SKUName,
    decimal UnitPrice,
    decimal MaximumDiscountPercent,
    decimal? MinimumOrderQuantity);

public sealed record PriceListListDto(
    int PriceListID,
    string PriceListCode,
    string PriceListName,
    SalesChannel Channel,
    int? ClientSegmentID,
    string? ClientSegmentName,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string CurrencyCode,
    PriceListStatus Status);

public sealed record PriceListDetailsDto(
    int PriceListID,
    string PriceListCode,
    string PriceListName,
    SalesChannel Channel,
    int? ClientSegmentID,
    string? ClientSegmentName,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    string CurrencyCode,
    PriceListStatus Status,
    IReadOnlyCollection<PriceListItemDto> Items);

public sealed class SavePriceListRequestDto
{
    public string PriceListCode { get; init; } = string.Empty;
    public string PriceListName { get; init; } = string.Empty;
    public SalesChannel Channel { get; init; }
    public int? ClientSegmentID { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public string CurrencyCode { get; init; } = "BDT";
    public IReadOnlyCollection<SavePriceListItemRequestDto> Items { get; init; } = Array.Empty<SavePriceListItemRequestDto>();
}

public sealed class SavePriceListItemRequestDto
{
    public int SKUID { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal MaximumDiscountPercent { get; init; }
    public decimal? MinimumOrderQuantity { get; init; }
}

public sealed class ChangePriceListStatusRequestDto
{
    public PriceListStatus Status { get; init; }
}

public sealed record StandardDiscountRuleDto(
    int StandardDiscountRuleID,
    string RuleName,
    SalesChannel Channel,
    int? ClientSegmentID,
    int? SKUID,
    int? ProductCategoryID,
    decimal? MinQuantity,
    decimal MaxDiscountPercent,
    bool RequiresApproval,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsActive);

public sealed class SaveStandardDiscountRuleRequestDto
{
    public string RuleName { get; init; } = string.Empty;
    public SalesChannel Channel { get; init; }
    public int? ClientSegmentID { get; init; }
    public int? SKUID { get; init; }
    public int? ProductCategoryID { get; init; }
    public decimal? MinQuantity { get; init; }
    public decimal MaxDiscountPercent { get; init; }
    public bool RequiresApproval { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PriceResolutionRequestDto
{
    public int SKUID { get; init; }
    public SalesChannel Channel { get; init; }
    public int? ClientSegmentID { get; init; }
    public decimal Quantity { get; init; }
    public DateOnly PriceDate { get; init; }
}

public sealed record PriceResolutionDto(
    int SKUID,
    int PriceListID,
    int PriceListItemID,
    decimal UnitPrice,
    decimal MaximumPriceListDiscountPercent,
    decimal MaximumStandardDiscountPercent,
    decimal EffectiveMaximumDiscountPercent,
    bool RequiresApproval,
    int? StandardDiscountRuleID,
    string CurrencyCode);

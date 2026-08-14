using MarketSphere.Domain.Enums;

namespace MarketSphere.Application.Modules.ProductPricing.DTOs;

public sealed record ProductCategoryListDto(
    int ProductCategoryID,
    int? ParentProductCategoryID,
    string CategoryCode,
    string CategoryName,
    ProductCategoryType CategoryType,
    bool IsActive);

public sealed record ProductCategoryDetailsDto(
    int ProductCategoryID,
    int? ParentProductCategoryID,
    string CategoryCode,
    string CategoryName,
    ProductCategoryType CategoryType,
    bool IsActive,
    IReadOnlyCollection<ProductCategoryListDto> Children);

public sealed class SaveProductCategoryRequestDto
{
    public int? ParentProductCategoryID { get; init; }
    public string CategoryCode { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public ProductCategoryType CategoryType { get; init; } = ProductCategoryType.Standard;
    public bool IsActive { get; init; } = true;
}

public sealed record BrandDto(
    int BrandID,
    string BrandCode,
    string BrandName,
    string? OwnerCompanyName,
    bool IsCustomerFacing,
    bool IsActive);

public sealed class SaveBrandRequestDto
{
    public string BrandCode { get; init; } = string.Empty;
    public string BrandName { get; init; } = string.Empty;
    public string? OwnerCompanyName { get; init; }
    public bool IsCustomerFacing { get; init; } = true;
    public bool IsActive { get; init; } = true;
}

public sealed record ProductListDto(
    int ProductID,
    string ProductCode,
    string ProductName,
    string CategoryName,
    string BrandName,
    ProductType ProductType,
    bool RequiresBatch,
    bool RequiresExpiryDate,
    bool IsActive);

public sealed record ProductDetailsDto(
    int ProductID,
    string ProductCode,
    int ProductCategoryID,
    string CategoryName,
    int BrandID,
    string BrandName,
    string ProductName,
    ProductType ProductType,
    string? Description,
    bool RequiresBatch,
    bool RequiresExpiryDate,
    bool IsActive,
    IReadOnlyCollection<SKUListDto> SKUs);

public sealed class SaveProductRequestDto
{
    public string ProductCode { get; init; } = string.Empty;
    public int ProductCategoryID { get; init; }
    public int BrandID { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public ProductType ProductType { get; init; } = ProductType.FinishedGood;
    public string? Description { get; init; }
    public bool RequiresBatch { get; init; }
    public bool RequiresExpiryDate { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed record SKUListDto(
    int SKUID,
    int ProductID,
    string ProductName,
    string SKUCode,
    string SKUName,
    string? Size,
    string Unit,
    string? Barcode,
    decimal MRP,
    decimal StandardTradePrice,
    bool IsActive);

public sealed record SKUDetailsDto(
    int SKUID,
    int ProductID,
    string ProductCode,
    string ProductName,
    string SKUCode,
    string SKUName,
    string? Size,
    string Unit,
    string? Barcode,
    decimal MRP,
    decimal StandardTradePrice,
    bool IsActive);

public sealed class SaveSKURequestDto
{
    public int ProductID { get; init; }
    public string SKUCode { get; init; } = string.Empty;
    public string SKUName { get; init; } = string.Empty;
    public string? Size { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal MRP { get; init; }
    public decimal StandardTradePrice { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class ChangeMasterStatusRequestDto
{
    public bool IsActive { get; init; }
}

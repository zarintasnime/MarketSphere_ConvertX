using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Mapping;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Application.Modules.ProductPricing.DTOs;
using MarketSphere.Application.Modules.ProductPricing.Interfaces;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.ProductPricing.Services;

public sealed class ProductService : IProductService
{
    private readonly IApplicationDbContext _db;

    public ProductService(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<ProductCategoryListDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
        => await _db.ProductCategories.AsNoTracking()
            .OrderBy(x => x.CategoryName)
            .Select(x => new ProductCategoryListDto(
                x.ProductCategoryID,
                x.ParentProductCategoryID,
                x.CategoryCode,
                x.CategoryName,
                x.CategoryType,
                x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<ProductCategoryDetailsDto> GetCategoryByIdAsync(
        int productCategoryID,
        CancellationToken cancellationToken = default)
    {
        var category = await _db.ProductCategories.AsNoTracking()
            .SingleOrDefaultAsync(x => x.ProductCategoryID == productCategoryID, cancellationToken)
            ?? throw new NotFoundException("Product category was not found.");

        var children = await _db.ProductCategories.AsNoTracking()
            .Where(x => x.ParentProductCategoryID == productCategoryID)
            .OrderBy(x => x.CategoryName)
            .Select(x => new ProductCategoryListDto(
                x.ProductCategoryID,
                x.ParentProductCategoryID,
                x.CategoryCode,
                x.CategoryName,
                x.CategoryType,
                x.IsActive))
            .ToListAsync(cancellationToken);

        return new ProductCategoryDetailsDto(
            category.ProductCategoryID,
            category.ParentProductCategoryID,
            category.CategoryCode,
            category.CategoryName,
            category.CategoryType,
            category.IsActive,
            children);
    }

    public async Task<int> CreateCategoryAsync(
        SaveProductCategoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateCategory(request);
        var code = request.CategoryCode.NormalizeCode();

        if (await _db.ProductCategories.AnyAsync(x => x.CategoryCode == code, cancellationToken))
            throw new ConflictException("Product category code already exists.");

        await ValidateParentCategoryAsync(null, request.ParentProductCategoryID, cancellationToken);

        var entity = new ProductCategory();
        ApplyCategory(entity, request, code);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.ProductCategoryID;
    }

    public async Task UpdateCategoryAsync(
        int productCategoryID,
        SaveProductCategoryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateCategory(request);
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.ProductCategories.Where(x => x.ProductCategoryID == productCategoryID),
            "Product category",
            cancellationToken);
        var code = request.CategoryCode.NormalizeCode();

        if (await _db.ProductCategories.AnyAsync(
                x => x.CategoryCode == code && x.ProductCategoryID != productCategoryID,
                cancellationToken))
            throw new ConflictException("Product category code already exists.");

        await ValidateParentCategoryAsync(productCategoryID, request.ParentProductCategoryID, cancellationToken);
        ApplyCategory(entity, request, code);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetCategoryStatusAsync(
        int productCategoryID,
        ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.ProductCategories.Where(x => x.ProductCategoryID == productCategoryID),
            "Product category",
            cancellationToken);

        if (!request.IsActive)
        {
            var hasActiveChildren = await _db.ProductCategories.AnyAsync(
                x => x.ParentProductCategoryID == productCategoryID && x.IsActive,
                cancellationToken);
            var hasActiveProducts = await _db.Products.AnyAsync(
                x => x.ProductCategoryID == productCategoryID && x.IsActive,
                cancellationToken);
            if (hasActiveChildren || hasActiveProducts)
                throw new BusinessRuleException("A category with active children or products cannot be deactivated.");
        }

        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<BrandDto>> GetBrandsAsync(
        CancellationToken cancellationToken = default)
        => await _db.Brands.AsNoTracking()
            .OrderBy(x => x.BrandName)
            .Select(x => new BrandDto(
                x.BrandID,
                x.BrandCode,
                x.BrandName,
                x.OwnerCompanyName,
                x.IsCustomerFacing,
                x.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<int> CreateBrandAsync(
        SaveBrandRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateBrand(request);
        var code = request.BrandCode.NormalizeCode();
        if (await _db.Brands.AnyAsync(x => x.BrandCode == code, cancellationToken))
            throw new ConflictException("Brand code already exists.");

        var entity = new Brand();
        ApplyBrand(entity, request, code);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.BrandID;
    }

    public async Task UpdateBrandAsync(
        int brandID,
        SaveBrandRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateBrand(request);
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.Brands.Where(x => x.BrandID == brandID),
            "Brand",
            cancellationToken);
        var code = request.BrandCode.NormalizeCode();
        if (await _db.Brands.AnyAsync(x => x.BrandCode == code && x.BrandID != brandID, cancellationToken))
            throw new ConflictException("Brand code already exists.");

        ApplyBrand(entity, request, code);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetBrandStatusAsync(
        int brandID,
        ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.Brands.Where(x => x.BrandID == brandID),
            "Brand",
            cancellationToken);
        if (!request.IsActive && await _db.Products.AnyAsync(x => x.BrandID == brandID && x.IsActive, cancellationToken))
            throw new BusinessRuleException("A brand with active products cannot be deactivated.");
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<PagedResult<ProductListDto>> GetProductsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.ProductCode.Contains(search) ||
                x.ProductName.Contains(search) ||
                x.ProductCategory.CategoryName.Contains(search) ||
                x.Brand.BrandName.Contains(search));
        }

        var projected = query.OrderBy(x => x.ProductName)
            .Select(x => new ProductListDto(
                x.ProductID,
                x.ProductCode,
                x.ProductName,
                x.ProductCategory.CategoryName,
                x.Brand.BrandName,
                x.ProductType,
                x.RequiresBatch,
                x.RequiresExpiryDate,
                x.IsActive));
        return ProductPricingServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<ProductDetailsDto> GetProductByIdAsync(
        int productID,
        CancellationToken cancellationToken = default)
    {
        var product = await _db.Products.AsNoTracking()
            .Where(x => x.ProductID == productID)
            .Select(x => new
            {
                Entity = x,
                CategoryName = x.ProductCategory.CategoryName,
                BrandName = x.Brand.BrandName
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Product was not found.");

        var skus = await _db.SKUs.AsNoTracking()
            .Where(x => x.ProductID == productID)
            .OrderBy(x => x.SKUName)
            .Select(x => new SKUListDto(
                x.SKUID,
                x.ProductID,
                x.Product.ProductName,
                x.SKUCode,
                x.SKUName,
                x.Size,
                x.Unit,
                x.Barcode,
                x.MRP,
                x.StandardTradePrice,
                x.IsActive))
            .ToListAsync(cancellationToken);

        var entity = product.Entity;
        return new ProductDetailsDto(
            entity.ProductID,
            entity.ProductCode,
            entity.ProductCategoryID,
            product.CategoryName,
            entity.BrandID,
            product.BrandName,
            entity.ProductName,
            entity.ProductType,
            entity.Description,
            entity.RequiresBatch,
            entity.RequiresExpiryDate,
            entity.IsActive,
            skus);
    }

    public async Task<int> CreateProductAsync(
        SaveProductRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateProduct(request);
        await ValidateProductReferencesAsync(request, cancellationToken);
        var code = request.ProductCode.NormalizeCode();
        if (await _db.Products.AnyAsync(x => x.ProductCode == code, cancellationToken))
            throw new ConflictException("Product code already exists.");

        var entity = new Product();
        ApplyProduct(entity, request, code);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.ProductID;
    }

    public async Task UpdateProductAsync(
        int productID,
        SaveProductRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateProduct(request);
        await ValidateProductReferencesAsync(request, cancellationToken);
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.Products.Where(x => x.ProductID == productID),
            "Product",
            cancellationToken);
        var code = request.ProductCode.NormalizeCode();
        if (await _db.Products.AnyAsync(x => x.ProductCode == code && x.ProductID != productID, cancellationToken))
            throw new ConflictException("Product code already exists.");

        ApplyProduct(entity, request, code);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetProductStatusAsync(
        int productID,
        ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.Products.Where(x => x.ProductID == productID),
            "Product",
            cancellationToken);
        if (!request.IsActive && await _db.SKUs.AnyAsync(x => x.ProductID == productID && x.IsActive, cancellationToken))
            throw new BusinessRuleException("A product with active SKUs cannot be deactivated.");
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<PagedResult<SKUListDto>> GetSKUsAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _db.SKUs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.SKUCode.Contains(search) ||
                x.SKUName.Contains(search) ||
                x.Product.ProductName.Contains(search) ||
                (x.Barcode != null && x.Barcode.Contains(search)));
        }

        var projected = query.OrderBy(x => x.SKUName)
            .Select(x => new SKUListDto(
                x.SKUID,
                x.ProductID,
                x.Product.ProductName,
                x.SKUCode,
                x.SKUName,
                x.Size,
                x.Unit,
                x.Barcode,
                x.MRP,
                x.StandardTradePrice,
                x.IsActive));
        return ProductPricingServiceHelper.ToPagedAsync(projected, request, cancellationToken);
    }

    public async Task<SKUDetailsDto> GetSKUByIdAsync(
        int skuID,
        CancellationToken cancellationToken = default)
        => await _db.SKUs.AsNoTracking()
            .Where(x => x.SKUID == skuID)
            .Select(x => new SKUDetailsDto(
                x.SKUID,
                x.ProductID,
                x.Product.ProductCode,
                x.Product.ProductName,
                x.SKUCode,
                x.SKUName,
                x.Size,
                x.Unit,
                x.Barcode,
                x.MRP,
                x.StandardTradePrice,
                x.IsActive))
            .SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException("SKU was not found.");

    public async Task<int> CreateSKUAsync(
        SaveSKURequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateSKU(request);
        await ValidateSKUProductAsync(request.ProductID, cancellationToken);
        var code = request.SKUCode.NormalizeCode();
        var barcode = request.Barcode.NullIfWhiteSpace();
        await ValidateSKUUniquenessAsync(null, code, barcode, cancellationToken);

        var entity = new SKU();
        ApplySKU(entity, request, code, barcode);
        await _db.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.SKUID;
    }

    public async Task UpdateSKUAsync(
        int skuID,
        SaveSKURequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateSKU(request);
        await ValidateSKUProductAsync(request.ProductID, cancellationToken);
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.SKUs.Where(x => x.SKUID == skuID),
            "SKU",
            cancellationToken);
        var code = request.SKUCode.NormalizeCode();
        var barcode = request.Barcode.NullIfWhiteSpace();
        await ValidateSKUUniquenessAsync(skuID, code, barcode, cancellationToken);
        ApplySKU(entity, request, code, barcode);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SetSKUStatusAsync(
        int skuID,
        ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await ProductPricingServiceHelper.RequireAsync(
            _db.SKUs.Where(x => x.SKUID == skuID),
            "SKU",
            cancellationToken);
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateParentCategoryAsync(
        int? currentCategoryID,
        int? parentCategoryID,
        CancellationToken cancellationToken)
    {
        if (!parentCategoryID.HasValue)
            return;
        if (currentCategoryID == parentCategoryID)
            throw new BusinessRuleException(BusinessRuleMessages.ProductCategoryCircularReference);

        var parentExists = await _db.ProductCategories.AnyAsync(
            x => x.ProductCategoryID == parentCategoryID && x.IsActive,
            cancellationToken);
        if (!parentExists)
            throw new NotFoundException("Active parent product category was not found.");

        if (!currentCategoryID.HasValue)
            return;

        var cursor = parentCategoryID;
        while (cursor.HasValue)
        {
            if (cursor == currentCategoryID)
                throw new BusinessRuleException(BusinessRuleMessages.ProductCategoryCircularReference);
            cursor = await _db.ProductCategories
                .Where(x => x.ProductCategoryID == cursor.Value)
                .Select(x => x.ParentProductCategoryID)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }

    private async Task ValidateProductReferencesAsync(
        SaveProductRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await _db.ProductCategories.AnyAsync(
                x => x.ProductCategoryID == request.ProductCategoryID && x.IsActive,
                cancellationToken))
            throw new NotFoundException("Active product category was not found.");
        if (!await _db.Brands.AnyAsync(x => x.BrandID == request.BrandID && x.IsActive, cancellationToken))
            throw new NotFoundException("Active brand was not found.");
    }

    private async Task ValidateSKUProductAsync(int productID, CancellationToken cancellationToken)
    {
        if (!await _db.Products.AnyAsync(x => x.ProductID == productID && x.IsActive, cancellationToken))
            throw new NotFoundException("Active product was not found.");
    }

    private async Task ValidateSKUUniquenessAsync(
        int? currentSKUID,
        string skuCode,
        string? barcode,
        CancellationToken cancellationToken)
    {
        if (await _db.SKUs.AnyAsync(
                x => x.SKUCode == skuCode && (!currentSKUID.HasValue || x.SKUID != currentSKUID),
                cancellationToken))
            throw new ConflictException("SKU code already exists.");
        if (barcode is not null && await _db.SKUs.AnyAsync(
                x => x.Barcode == barcode && (!currentSKUID.HasValue || x.SKUID != currentSKUID),
                cancellationToken))
            throw new ConflictException("Barcode already exists.");
    }

    private static void ValidateCategory(SaveProductCategoryRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.CategoryCode, nameof(request.CategoryCode), 30);
        ValidationHelper.RequireNotBlank(request.CategoryName, nameof(request.CategoryName), 150);
    }

    private static void ApplyCategory(
        ProductCategory entity,
        SaveProductCategoryRequestDto request,
        string code)
    {
        entity.ParentProductCategoryID = request.ParentProductCategoryID;
        entity.CategoryCode = code;
        entity.CategoryName = request.CategoryName.Trim();
        entity.CategoryType = request.CategoryType;
        entity.IsActive = request.IsActive;
    }

    private static void ValidateBrand(SaveBrandRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.BrandCode, nameof(request.BrandCode), 30);
        ValidationHelper.RequireNotBlank(request.BrandName, nameof(request.BrandName), 150);
    }

    private static void ApplyBrand(Brand entity, SaveBrandRequestDto request, string code)
    {
        entity.BrandCode = code;
        entity.BrandName = request.BrandName.Trim();
        entity.OwnerCompanyName = request.OwnerCompanyName.NullIfWhiteSpace();
        entity.IsCustomerFacing = request.IsCustomerFacing;
        entity.IsActive = request.IsActive;
    }

    private static void ValidateProduct(SaveProductRequestDto request)
    {
        ValidationHelper.RequireNotBlank(request.ProductCode, nameof(request.ProductCode), 40);
        ValidationHelper.RequireNotBlank(request.ProductName, nameof(request.ProductName), 200);
        ValidationHelper.Require(request.ProductCategoryID > 0, nameof(request.ProductCategoryID), "ProductCategoryID must be greater than zero.");
        ValidationHelper.Require(request.BrandID > 0, nameof(request.BrandID), "BrandID must be greater than zero.");
        if (request.RequiresExpiryDate && !request.RequiresBatch)
            throw new BusinessRuleException(BusinessRuleMessages.ProductExpiryRequiresBatch);
    }

    private static void ApplyProduct(Product entity, SaveProductRequestDto request, string code)
    {
        entity.ProductCode = code;
        entity.ProductCategoryID = request.ProductCategoryID;
        entity.BrandID = request.BrandID;
        entity.ProductName = request.ProductName.Trim();
        entity.ProductType = request.ProductType;
        entity.Description = request.Description.NullIfWhiteSpace();
        entity.RequiresBatch = request.RequiresBatch;
        entity.RequiresExpiryDate = request.RequiresExpiryDate;
        entity.IsActive = request.IsActive;
    }

    private static void ValidateSKU(SaveSKURequestDto request)
    {
        ValidationHelper.Require(request.ProductID > 0, nameof(request.ProductID), "ProductID must be greater than zero.");
        ValidationHelper.RequireNotBlank(request.SKUCode, nameof(request.SKUCode), 50);
        ValidationHelper.RequireNotBlank(request.SKUName, nameof(request.SKUName), 200);
        ValidationHelper.RequireNotBlank(request.Unit, nameof(request.Unit), 30);
        ValidationHelper.Require(request.MRP >= 0, nameof(request.MRP), "MRP cannot be negative.");
        ValidationHelper.Require(request.StandardTradePrice >= 0, nameof(request.StandardTradePrice), "Standard trade price cannot be negative.");
    }

    private static void ApplySKU(SKU entity, SaveSKURequestDto request, string code, string? barcode)
    {
        entity.ProductID = request.ProductID;
        entity.SKUCode = code;
        entity.SKUName = request.SKUName.Trim();
        entity.Size = request.Size.NullIfWhiteSpace();
        entity.Unit = request.Unit.Trim();
        entity.Barcode = barcode;
        entity.MRP = request.MRP;
        entity.StandardTradePrice = request.StandardTradePrice;
        entity.IsActive = request.IsActive;
    }
}

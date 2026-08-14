using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Domain.Entities.ProductPricing;
using MarketSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Infrastructure.Persistence.Seeders;

public sealed class ProductPricingSeeder
{
    private readonly MarketSphereDbContext _db;
    private readonly IDateTimeProvider _clock;

    public ProductPricingSeeder(MarketSphereDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var category = await _db.ProductCategories.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.CategoryCode == "BEVERAGES", cancellationToken);
        if (category is null)
        {
            category = new ProductCategory
            {
                CategoryCode = "BEVERAGES",
                CategoryName = "Beverages",
                CategoryType = ProductCategoryType.Standard,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };
            await _db.ProductCategories.AddAsync(category, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            category.CategoryName = "Beverages";
            category.CategoryType = ProductCategoryType.Standard;
            category.IsActive = true;
            category.IsDeleted = false;
        }

        var brand = await _db.Brands.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.BrandCode == "MARKETSPHERE", cancellationToken);
        if (brand is null)
        {
            brand = new Brand
            {
                BrandCode = "MARKETSPHERE",
                BrandName = "MarketSphere Bangladesh",
                OwnerCompanyName = "MarketSphere Distribution Bangladesh Ltd.",
                IsCustomerFacing = true,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };
            await _db.Brands.AddAsync(brand, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            brand.BrandName = "MarketSphere Bangladesh";
            brand.OwnerCompanyName = "MarketSphere Distribution Bangladesh Ltd.";
            brand.IsCustomerFacing = true;
            brand.IsActive = true;
            brand.IsDeleted = false;
        }

        var batchProduct = await EnsureProductAsync(
            "DEMO-BATCH",
            "Mango Drink 250ml",
            category.ProductCategoryID,
            brand.BrandID,
            true,
            true,
            cancellationToken);
        var nonBatchProduct = await EnsureProductAsync(
            "DEMO-NONBATCH",
            "Mineral Water 500ml",
            category.ProductCategoryID,
            brand.BrandID,
            false,
            false,
            cancellationToken);

        var batchSKU = await EnsureSKUAsync(
            batchProduct.ProductID,
            "DEMO-BATCH-001",
            "Mango Drink 250ml Bottle",
            "PCS",
            35m,
            30m,
            cancellationToken);
        var nonBatchSKU = await EnsureSKUAsync(
            nonBatchProduct.ProductID,
            "DEMO-NONBATCH-001",
            "Mineral Water 500ml Bottle",
            "PCS",
            25m,
            21m,
            cancellationToken);

        var priceList = await _db.PriceLists.SingleOrDefaultAsync(
            x => x.PriceListCode == "DEFAULT-GT",
            cancellationToken);
        if (priceList is null)
        {
            priceList = new PriceList
            {
                PriceListCode = "DEFAULT-GT",
                PriceListName = "Default GT Price List",
                Channel = SalesChannel.GeneralTrade,
                EffectiveFrom = new DateOnly(2026, 1, 1),
                CurrencyCode = "BDT",
                Status = PriceListStatus.Active,
                CreatedAt = _clock.UtcNow
            };
            await _db.PriceLists.AddAsync(priceList, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            priceList.PriceListName = "Default GT Price List";
            priceList.Channel = SalesChannel.GeneralTrade;
            priceList.EffectiveFrom = new DateOnly(2026, 1, 1);
            priceList.CurrencyCode = "BDT";
            priceList.Status = PriceListStatus.Active;
        }

        await EnsurePriceListItemAsync(priceList.PriceListID, batchSKU.SKUID, 30m, 8m, cancellationToken);
        await EnsurePriceListItemAsync(priceList.PriceListID, nonBatchSKU.SKUID, 21m, 8m, cancellationToken);

        var litchiProduct = await EnsureProductAsync(
            "MSX-LITCHI",
            "Litchi Drink 250ml",
            category.ProductCategoryID,
            brand.BrandID,
            true,
            true,
            cancellationToken);
        var energyProduct = await EnsureProductAsync(
            "MSX-ENERGY",
            "Energy Drink 250ml",
            category.ProductCategoryID,
            brand.BrandID,
            false,
            false,
            cancellationToken);

        var litchiSKU = await EnsureSKUAsync(
            litchiProduct.ProductID,
            "MSX-LITCHI-250",
            "Litchi Drink 250ml Can",
            "PCS",
            40m,
            34m,
            cancellationToken);
        var energySKU = await EnsureSKUAsync(
            energyProduct.ProductID,
            "MSX-ENERGY-250",
            "Energy Drink 250ml Can",
            "PCS",
            55m,
            47m,
            cancellationToken);

        await EnsurePriceListItemAsync(priceList.PriceListID, litchiSKU.SKUID, 34m, 8m, cancellationToken);
        await EnsurePriceListItemAsync(priceList.PriceListID, energySKU.SKUID, 47m, 8m, cancellationToken);

        if (!await _db.StandardDiscountRules.AnyAsync(x => x.RuleName == "Default GT Discount", cancellationToken))
        {
            await _db.StandardDiscountRules.AddAsync(new StandardDiscountRule
            {
                RuleName = "Default GT Discount",
                Channel = SalesChannel.GeneralTrade,
                MinQuantity = 1m,
                MaxDiscountPercent = 5m,
                RequiresApproval = false,
                EffectiveFrom = new DateOnly(2026, 1, 1),
                IsActive = true,
                CreatedAt = _clock.UtcNow
            }, cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> EnsureProductAsync(
        string code,
        string name,
        int categoryID,
        int brandID,
        bool requiresBatch,
        bool requiresExpiry,
        CancellationToken cancellationToken)
    {
        var product = await _db.Products.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.ProductCode == code, cancellationToken);

        if (product is null)
        {
            product = new Product
            {
                ProductCode = code,
                ProductName = name,
                ProductCategoryID = categoryID,
                BrandID = brandID,
                ProductType = ProductType.FinishedGood,
                RequiresBatch = requiresBatch,
                RequiresExpiryDate = requiresExpiry,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };
            await _db.Products.AddAsync(product, cancellationToken);
        }
        else
        {
            product.ProductName = name;
            product.ProductCategoryID = categoryID;
            product.BrandID = brandID;
            product.ProductType = ProductType.FinishedGood;
            product.RequiresBatch = requiresBatch;
            product.RequiresExpiryDate = requiresExpiry;
            product.IsActive = true;
            product.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return product;
    }

    private async Task<SKU> EnsureSKUAsync(
        int productID,
        string code,
        string name,
        string unit,
        decimal mrp,
        decimal tradePrice,
        CancellationToken cancellationToken)
    {
        var sku = await _db.SKUs.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.SKUCode == code, cancellationToken);

        if (sku is null)
        {
            sku = new SKU
            {
                ProductID = productID,
                SKUCode = code,
                SKUName = name,
                Unit = unit,
                MRP = mrp,
                StandardTradePrice = tradePrice,
                IsActive = true,
                CreatedAt = _clock.UtcNow
            };
            await _db.SKUs.AddAsync(sku, cancellationToken);
        }
        else
        {
            sku.ProductID = productID;
            sku.SKUName = name;
            sku.Unit = unit;
            sku.MRP = mrp;
            sku.StandardTradePrice = tradePrice;
            sku.IsActive = true;
            sku.IsDeleted = false;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return sku;
    }

    private async Task EnsurePriceListItemAsync(
        int priceListID,
        int skuID,
        decimal unitPrice,
        decimal maximumDiscount,
        CancellationToken cancellationToken)
    {
        var item = await _db.PriceListItems
            .SingleOrDefaultAsync(
                x => x.PriceListID == priceListID && x.SKUID == skuID,
                cancellationToken);

        if (item is null)
        {
            await _db.PriceListItems.AddAsync(new PriceListItem
            {
                PriceListID = priceListID,
                SKUID = skuID,
                UnitPrice = unitPrice,
                MaximumDiscountPercent = maximumDiscount,
                MinimumOrderQuantity = 1m,
                CreatedAt = _clock.UtcNow
            }, cancellationToken);
            return;
        }

        item.UnitPrice = unitPrice;
        item.MaximumDiscountPercent = maximumDiscount;
        item.MinimumOrderQuantity = 1m;
    }
}

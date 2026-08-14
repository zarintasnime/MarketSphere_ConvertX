using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.ProductPricing.DTOs;

namespace MarketSphere.Application.Modules.ProductPricing.Interfaces;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductCategoryListDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ProductCategoryDetailsDto> GetCategoryByIdAsync(int productCategoryID, CancellationToken cancellationToken = default);
    Task<int> CreateCategoryAsync(SaveProductCategoryRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(int productCategoryID, SaveProductCategoryRequestDto request, CancellationToken cancellationToken = default);
    Task SetCategoryStatusAsync(int productCategoryID, ChangeMasterStatusRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BrandDto>> GetBrandsAsync(CancellationToken cancellationToken = default);
    Task<int> CreateBrandAsync(SaveBrandRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateBrandAsync(int brandID, SaveBrandRequestDto request, CancellationToken cancellationToken = default);
    Task SetBrandStatusAsync(int brandID, ChangeMasterStatusRequestDto request, CancellationToken cancellationToken = default);

    Task<PagedResult<ProductListDto>> GetProductsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ProductDetailsDto> GetProductByIdAsync(int productID, CancellationToken cancellationToken = default);
    Task<int> CreateProductAsync(SaveProductRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(int productID, SaveProductRequestDto request, CancellationToken cancellationToken = default);
    Task SetProductStatusAsync(int productID, ChangeMasterStatusRequestDto request, CancellationToken cancellationToken = default);

    Task<PagedResult<SKUListDto>> GetSKUsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<SKUDetailsDto> GetSKUByIdAsync(int skuID, CancellationToken cancellationToken = default);
    Task<int> CreateSKUAsync(SaveSKURequestDto request, CancellationToken cancellationToken = default);
    Task UpdateSKUAsync(int skuID, SaveSKURequestDto request, CancellationToken cancellationToken = default);
    Task SetSKUStatusAsync(int skuID, ChangeMasterStatusRequestDto request, CancellationToken cancellationToken = default);
}

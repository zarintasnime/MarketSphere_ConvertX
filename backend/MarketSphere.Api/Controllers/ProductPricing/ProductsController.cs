using MarketSphere.Api.Authorization;
using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.ProductPricing.DTOs;
using MarketSphere.Application.Modules.ProductPricing.Interfaces;
using MarketSphere.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketSphere.Api.Controllers.ProductPricing;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet("categories")]
    [HasPermission(PermissionCodes.ProductCategoriesView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<ProductCategoryListDto>>>> GetCategories(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetCategoriesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<ProductCategoryListDto>>.Success(result, "Product categories retrieved successfully."));
    }

    [HttpGet("categories/{productCategoryID:int}")]
    [HasPermission(PermissionCodes.ProductCategoriesView)]
    public async Task<ActionResult<ApiResponse<ProductCategoryDetailsDto>>> GetCategoryById(
            int productCategoryID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetCategoryByIdAsync(productCategoryID, cancellationToken);
        return Ok(ApiResponse<ProductCategoryDetailsDto>.Success(result, "Product category retrieved successfully."));
    }

    [HttpPost("categories")]
    [HasPermission(PermissionCodes.ProductCategoriesManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateCategory(
        [FromBody] SaveProductCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateCategoryAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Product category created successfully."));
    }

    [HttpPut("categories/{productCategoryID:int}")]
    [HasPermission(PermissionCodes.ProductCategoriesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateCategory(
        int productCategoryID,
        [FromBody] SaveProductCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateCategoryAsync(productCategoryID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Product category updated successfully."));
    }

    [HttpPatch("categories/{productCategoryID:int}/status")]
    [HasPermission(PermissionCodes.ProductCategoriesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> SetCategoryStatus(
        int productCategoryID,
        [FromBody] ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SetCategoryStatusAsync(productCategoryID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Product category status changed successfully."));
    }

    [HttpGet("brands")]
    [HasPermission(PermissionCodes.BrandsView)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<BrandDto>>>> GetBrands(
            CancellationToken cancellationToken)
    {
        var result = await _service.GetBrandsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<BrandDto>>.Success(result, "Brands retrieved successfully."));
    }

    [HttpPost("brands")]
    [HasPermission(PermissionCodes.BrandsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateBrand(
        [FromBody] SaveBrandRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateBrandAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Brand created successfully."));
    }

    [HttpPut("brands/{brandID:int}")]
    [HasPermission(PermissionCodes.BrandsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBrand(
        int brandID,
        [FromBody] SaveBrandRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateBrandAsync(brandID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Brand updated successfully."));
    }

    [HttpPatch("brands/{brandID:int}/status")]
    [HasPermission(PermissionCodes.BrandsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> SetBrandStatus(
        int brandID,
        [FromBody] ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SetBrandStatusAsync(brandID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Brand status changed successfully."));
    }

    [HttpGet("products")]
    [HasPermission(PermissionCodes.ProductsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductListDto>>>> GetProducts(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetProductsAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<ProductListDto>>.Success(result, "Products retrieved successfully."));
    }

    [HttpGet("{productID:int}")]
    [HasPermission(PermissionCodes.ProductsView)]
    public async Task<ActionResult<ApiResponse<ProductDetailsDto>>> GetProductById(
            int productID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetProductByIdAsync(productID, cancellationToken);
        return Ok(ApiResponse<ProductDetailsDto>.Success(result, "Product retrieved successfully."));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.ProductsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateProduct(
        [FromBody] SaveProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateProductAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Product created successfully."));
    }

    [HttpPut("{productID:int}")]
    [HasPermission(PermissionCodes.ProductsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateProduct(
        int productID,
        [FromBody] SaveProductRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateProductAsync(productID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Product updated successfully."));
    }

    [HttpPatch("{productID:int}/status")]
    [HasPermission(PermissionCodes.ProductsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> SetProductStatus(
        int productID,
        [FromBody] ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SetProductStatusAsync(productID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Product status changed successfully."));
    }

    [HttpGet("skus")]
    [HasPermission(PermissionCodes.SKUsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<SKUListDto>>>> GetSKUs(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetSKUsAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<SKUListDto>>.Success(result, "SKUs retrieved successfully."));
    }

    [HttpGet("skus/{skuID:int}")]
    [HasPermission(PermissionCodes.SKUsView)]
    public async Task<ActionResult<ApiResponse<SKUDetailsDto>>> GetSKUById(
            int skuID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetSKUByIdAsync(skuID, cancellationToken);
        return Ok(ApiResponse<SKUDetailsDto>.Success(result, "SKU retrieved successfully."));
    }

    [HttpPost("skus")]
    [HasPermission(PermissionCodes.SKUsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateSKU(
        [FromBody] SaveSKURequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateSKUAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "SKU created successfully."));
    }

    [HttpPut("skus/{skuID:int}")]
    [HasPermission(PermissionCodes.SKUsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateSKU(
        int skuID,
        [FromBody] SaveSKURequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateSKUAsync(skuID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "SKU updated successfully."));
    }

    [HttpPatch("skus/{skuID:int}/status")]
    [HasPermission(PermissionCodes.SKUsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> SetSKUStatus(
        int skuID,
        [FromBody] ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SetSKUStatusAsync(skuID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "SKU status changed successfully."));
    }
}

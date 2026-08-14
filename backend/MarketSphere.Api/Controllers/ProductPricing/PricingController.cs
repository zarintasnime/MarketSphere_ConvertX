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
[Route("api/pricing")]
public sealed class PricingController : ControllerBase
{
    private readonly IPricingService _service;

    public PricingController(IPricingService service) => _service = service;

    [HttpGet("price-lists")]
    [HasPermission(PermissionCodes.PriceListsView)]
    public async Task<ActionResult<ApiResponse<PagedResult<PriceListListDto>>>> GetPriceLists(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPriceListsAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PriceListListDto>>.Success(result, "Price lists retrieved successfully."));
    }

    [HttpGet("price-lists/{priceListID:int}")]
    [HasPermission(PermissionCodes.PriceListsView)]
    public async Task<ActionResult<ApiResponse<PriceListDetailsDto>>> GetPriceListById(
            int priceListID,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetPriceListByIdAsync(priceListID, cancellationToken);
        return Ok(ApiResponse<PriceListDetailsDto>.Success(result, "Price list retrieved successfully."));
    }

    [HttpPost("price-lists")]
    [HasPermission(PermissionCodes.PriceListsManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreatePriceList(
        [FromBody] SavePriceListRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreatePriceListAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Price list created successfully."));
    }

    [HttpPut("price-lists/{priceListID:int}")]
    [HasPermission(PermissionCodes.PriceListsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePriceList(
        int priceListID,
        [FromBody] SavePriceListRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdatePriceListAsync(priceListID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Price list updated successfully."));
    }

    [HttpPatch("price-lists/{priceListID:int}/status")]
    [HasPermission(PermissionCodes.PriceListsManage)]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePriceListStatus(
        int priceListID,
        [FromBody] ChangePriceListStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.ChangePriceListStatusAsync(priceListID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Price list status changed successfully."));
    }

    [HttpGet("discount-rules")]
    [HasPermission(PermissionCodes.DiscountRulesView)]
    public async Task<ActionResult<ApiResponse<PagedResult<StandardDiscountRuleDto>>>> GetDiscountRules(
            [FromQuery] PagedRequest request,
            CancellationToken cancellationToken)
    {
        var result = await _service.GetDiscountRulesAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<StandardDiscountRuleDto>>.Success(result, "Discount rules retrieved successfully."));
    }

    [HttpPost("discount-rules")]
    [HasPermission(PermissionCodes.DiscountRulesManage)]
    public async Task<ActionResult<ApiResponse<int>>> CreateDiscountRule(
        [FromBody] SaveStandardDiscountRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        var id = await _service.CreateDiscountRuleAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<int>.Success(id, "Discount rule created successfully."));
    }

    [HttpPut("discount-rules/{standardDiscountRuleID:int}")]
    [HasPermission(PermissionCodes.DiscountRulesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateDiscountRule(
        int standardDiscountRuleID,
        [FromBody] SaveStandardDiscountRuleRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.UpdateDiscountRuleAsync(standardDiscountRuleID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Discount rule updated successfully."));
    }

    [HttpPatch("discount-rules/{standardDiscountRuleID:int}/status")]
    [HasPermission(PermissionCodes.DiscountRulesManage)]
    public async Task<ActionResult<ApiResponse<bool>>> SetDiscountRuleStatus(
        int standardDiscountRuleID,
        [FromBody] ChangeMasterStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        await _service.SetDiscountRuleStatusAsync(standardDiscountRuleID, request, cancellationToken);
        return Ok(ApiResponse<bool>.Success(true, "Discount rule status changed successfully."));
    }

    [HttpPost("resolve")]
    [HasPermission(PermissionCodes.PriceListsView)]
    public async Task<ActionResult<ApiResponse<PriceResolutionDto>>> ResolvePrice(
            [FromBody] PriceResolutionRequestDto request,
            CancellationToken cancellationToken)
    {
        var result = await _service.ResolvePriceAsync(request, cancellationToken);
        return Ok(ApiResponse<PriceResolutionDto>.Success(result, "Price resolved successfully."));
    }
}

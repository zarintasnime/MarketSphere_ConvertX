using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.ProductPricing.DTOs;

namespace MarketSphere.Application.Modules.ProductPricing.Interfaces;

public interface IPricingService
{
    Task<PagedResult<PriceListListDto>> GetPriceListsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<PriceListDetailsDto> GetPriceListByIdAsync(int priceListID, CancellationToken cancellationToken = default);
    Task<int> CreatePriceListAsync(SavePriceListRequestDto request, CancellationToken cancellationToken = default);
    Task UpdatePriceListAsync(int priceListID, SavePriceListRequestDto request, CancellationToken cancellationToken = default);
    Task ChangePriceListStatusAsync(int priceListID, ChangePriceListStatusRequestDto request, CancellationToken cancellationToken = default);

    Task<PagedResult<StandardDiscountRuleDto>> GetDiscountRulesAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<int> CreateDiscountRuleAsync(SaveStandardDiscountRuleRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateDiscountRuleAsync(int standardDiscountRuleID, SaveStandardDiscountRuleRequestDto request, CancellationToken cancellationToken = default);
    Task SetDiscountRuleStatusAsync(int standardDiscountRuleID, ChangeMasterStatusRequestDto request, CancellationToken cancellationToken = default);

    Task<PriceResolutionDto> ResolvePriceAsync(PriceResolutionRequestDto request, CancellationToken cancellationToken = default);
}

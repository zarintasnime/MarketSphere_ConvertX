using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface ICampaignService
{
    Task<PagedResult<CampaignListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<CampaignDetailsDto> GetByIdAsync(int campaignID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveCampaignRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int campaignID, SaveCampaignRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int campaignID, ChangeCampaignStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<int> AddTargetAsync(int campaignID, SaveCampaignTargetRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateTargetAsync(int campaignTargetID, SaveCampaignTargetRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteTargetAsync(int campaignTargetID, CancellationToken cancellationToken = default);
    Task<int> AddOfferAsync(int campaignID, SaveCampaignOfferRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateOfferAsync(int campaignOfferID, SaveCampaignOfferRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteOfferAsync(int campaignOfferID, CancellationToken cancellationToken = default);
    Task<int> AddExpenseAsync(int campaignID, SaveCampaignExpenseRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateExpenseAsync(int campaignExpenseID, SaveCampaignExpenseRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeExpenseStatusAsync(int campaignExpenseID, ChangeCampaignExpenseStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<int> AddAttributionAsync(int campaignID, SaveCampaignAttributionRequestDto request, CancellationToken cancellationToken = default);
    Task<CampaignRoiDto> GetRoiAsync(int campaignID, CancellationToken cancellationToken = default);
}

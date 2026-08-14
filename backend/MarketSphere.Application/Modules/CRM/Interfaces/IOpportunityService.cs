using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface IOpportunityService
{
    Task<PagedResult<OpportunityListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<OpportunityDetailsDto> GetByIdAsync(int opportunityID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveOpportunityRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int opportunityID, SaveOpportunityRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStageAsync(int opportunityID, ChangeOpportunityStageRequestDto request, CancellationToken cancellationToken = default);
}

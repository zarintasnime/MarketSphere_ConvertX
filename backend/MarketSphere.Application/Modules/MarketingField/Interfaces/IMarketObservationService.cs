using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface IMarketObservationService
{
    Task<PagedResult<MarketObservationListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<MarketObservationDetailsDto> GetByIdAsync(int marketObservationID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveMarketObservationRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int marketObservationID, SaveMarketObservationRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int marketObservationID, CancellationToken cancellationToken = default);
}

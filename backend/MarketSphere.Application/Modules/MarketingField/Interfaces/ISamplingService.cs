using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface ISamplingService
{
    Task<PagedResult<SamplingLogListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<SamplingLogDetailsDto> GetByIdAsync(int samplingLogID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveSamplingLogRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int samplingLogID, SaveSamplingLogRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int samplingLogID, CancellationToken cancellationToken = default);
}

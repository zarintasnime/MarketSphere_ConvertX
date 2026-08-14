using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface IBPSellOutService
{
    Task<PagedResult<BPSellOutListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<BPSellOutDetailsDto> GetByIdAsync(int bpSellOutID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveBPSellOutRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int bpSellOutID, SaveBPSellOutRequestDto request, CancellationToken cancellationToken = default);
    Task VerifyAsync(int bpSellOutID, VerifyBPSellOutRequestDto request, CancellationToken cancellationToken = default);
}

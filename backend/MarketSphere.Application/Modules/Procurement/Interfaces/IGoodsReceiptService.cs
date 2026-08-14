using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
namespace MarketSphere.Application.Modules.Procurement.Interfaces;
public interface IGoodsReceiptService
{
    Task<PagedResult<GoodsReceiptListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<GoodsReceiptDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveGoodsReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SaveGoodsReceiptRequestDto request, CancellationToken cancellationToken = default);
    Task CompleteQualityCheckAsync(int id, CompleteQualityCheckRequestDto request, CancellationToken cancellationToken = default);
    Task PostAsync(int id, PostGoodsReceiptRequestDto request, CancellationToken cancellationToken = default);
}

using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.DTOs;
namespace MarketSphere.Application.Modules.Inventory.Interfaces;
public interface IStockAdjustmentService
{
    Task<PagedResult<StockAdjustmentListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<StockAdjustmentDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveStockAdjustmentRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SaveStockAdjustmentRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangeStockAdjustmentStatusRequestDto request, CancellationToken cancellationToken = default);
    Task PostAsync(int id, PostStockAdjustmentRequestDto request, CancellationToken cancellationToken = default);
}

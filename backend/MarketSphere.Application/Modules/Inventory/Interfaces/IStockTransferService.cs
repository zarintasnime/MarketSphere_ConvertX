using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Inventory.DTOs;
namespace MarketSphere.Application.Modules.Inventory.Interfaces;
public interface IStockTransferService
{
    Task<PagedResult<StockTransferListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<StockTransferDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveStockTransferRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SaveStockTransferRequestDto request, CancellationToken cancellationToken = default);
    Task SubmitAsync(int id, CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task DispatchAsync(int id, DispatchStockTransferRequestDto request, CancellationToken cancellationToken = default);
    Task ReceiveAsync(int id, ReceiveStockTransferRequestDto request, CancellationToken cancellationToken = default);
}

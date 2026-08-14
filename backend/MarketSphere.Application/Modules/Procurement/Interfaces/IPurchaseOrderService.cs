using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
namespace MarketSphere.Application.Modules.Procurement.Interfaces;
public interface IPurchaseOrderService
{
    Task<PagedResult<PurchaseOrderListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<PurchaseOrderDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SavePurchaseOrderRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SavePurchaseOrderRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangePurchaseOrderStatusRequestDto request, CancellationToken cancellationToken = default);
}

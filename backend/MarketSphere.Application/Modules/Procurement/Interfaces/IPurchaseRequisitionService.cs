using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
namespace MarketSphere.Application.Modules.Procurement.Interfaces;
public interface IPurchaseRequisitionService
{
    Task<PagedResult<PurchaseRequisitionListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<PurchaseRequisitionDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SavePurchaseRequisitionRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SavePurchaseRequisitionRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangePurchaseRequisitionStatusRequestDto request, CancellationToken cancellationToken = default);
}

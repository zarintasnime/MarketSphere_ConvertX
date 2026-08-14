using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
namespace MarketSphere.Application.Modules.Procurement.Interfaces;
public interface ISupplierReturnService
{
    Task<PagedResult<SupplierReturnListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<SupplierReturnDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveSupplierReturnRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SaveSupplierReturnRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangeSupplierReturnStatusRequestDto request, CancellationToken cancellationToken = default);
    Task PostAsync(int id, PostSupplierReturnRequestDto request, CancellationToken cancellationToken = default);
}

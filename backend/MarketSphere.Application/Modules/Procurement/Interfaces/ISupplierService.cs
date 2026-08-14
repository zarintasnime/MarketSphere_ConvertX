using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Procurement.DTOs;
namespace MarketSphere.Application.Modules.Procurement.Interfaces;
public interface ISupplierService
{
    Task<PagedResult<SupplierListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<SupplierDetailsDto> GetByIdAsync(int supplierID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveSupplierRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int supplierID, SaveSupplierRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int supplierID, ChangeSupplierStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<int> UpsertProductAsync(int supplierID, SaveSupplierProductRequestDto request, CancellationToken cancellationToken = default);
}

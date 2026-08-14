using MarketSphere.Application.Modules.Inventory.DTOs;
namespace MarketSphere.Application.Modules.Inventory.Interfaces;
public interface IWarehouseService
{
    Task<IReadOnlyCollection<WarehouseDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveWarehouseRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SaveWarehouseRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangeWarehouseStatusRequestDto request, CancellationToken cancellationToken = default);
}

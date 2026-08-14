using MarketSphere.Application.Modules.Inventory.DTOs;
namespace MarketSphere.Application.Modules.Inventory.Interfaces;
public interface IStockService
{
    Task<IReadOnlyCollection<StockBalanceDto>> GetBalancesAsync(StockSearchRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<BatchDto>> GetBatchesAsync(int? skuID, bool includeExpired, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StockMovementDto>> GetMovementsAsync(int? warehouseID, int? skuID, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<StockReservationDto>> GetReservationsAsync(int? orderItemID, CancellationToken cancellationToken = default);
}

using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
namespace MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
public interface IAppliedOfferService { Task<IReadOnlyCollection<AppliedOfferDto>> GetForOrderAsync(int orderID, CancellationToken cancellationToken = default); Task<int> ApplyAsync(ApplyOfferRequestDto request, CancellationToken cancellationToken = default); Task RemoveAsync(int id, CancellationToken cancellationToken = default); }

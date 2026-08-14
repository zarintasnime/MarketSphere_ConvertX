using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
namespace MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
public interface IDeliveryService { Task<PagedResult<DeliveryListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default); Task<DeliveryDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default); Task<int> CreateAsync(CreateDeliveryRequestDto request, CancellationToken cancellationToken = default); Task DispatchAsync(int id, DispatchDeliveryRequestDto request, CancellationToken cancellationToken = default); Task CompleteAsync(int id, CompleteDeliveryRequestDto request, CancellationToken cancellationToken = default); }

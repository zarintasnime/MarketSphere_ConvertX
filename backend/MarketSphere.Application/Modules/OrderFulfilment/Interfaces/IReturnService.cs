using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.OrderFulfilment.DTOs;
namespace MarketSphere.Application.Modules.OrderFulfilment.Interfaces;
public interface IReturnService { Task<PagedResult<ReturnListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default); Task<ReturnDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default); Task<int> CreateAsync(CreateReturnRequestDto request, CancellationToken cancellationToken = default); Task ApproveAsync(int id, ApproveReturnRequestDto request, CancellationToken cancellationToken = default); Task ResolveAsync(int id, ResolveReturnRequestDto request, CancellationToken cancellationToken = default); }

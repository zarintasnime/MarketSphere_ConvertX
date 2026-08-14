using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface IReactivationService
{
    Task<PagedResult<ReactivationCaseDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ReactivationCaseDto> GetByIdAsync(int reactivationCaseID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateReactivationCaseRequestDto request, CancellationToken cancellationToken = default);
    Task ResolveAsync(int reactivationCaseID, ResolveReactivationCaseRequestDto request, CancellationToken cancellationToken = default);
}

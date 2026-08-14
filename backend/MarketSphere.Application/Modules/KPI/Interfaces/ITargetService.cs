using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;

namespace MarketSphere.Application.Modules.KPI.Interfaces;

public interface ITargetService
{
    Task<PagedResult<EmployeeTargetListDto>> GetAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<EmployeeTargetDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveEmployeeTargetRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, SaveEmployeeTargetRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangeEmployeeTargetStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<TargetProgressDto> GetProgressAsync(int id, CancellationToken cancellationToken = default);
}

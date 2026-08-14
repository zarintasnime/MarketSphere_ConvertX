using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface ICrmTaskService
{
    Task<PagedResult<CrmTaskDto>> GetPagedAsync(PagedRequest request, int? assignedEmployeeID, bool overdueOnly, CancellationToken cancellationToken = default);
    Task<CrmTaskDto> GetByIdAsync(int taskID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveCrmTaskRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int taskID, SaveCrmTaskRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int taskID, ChangeCrmTaskStatusRequestDto request, CancellationToken cancellationToken = default);
}

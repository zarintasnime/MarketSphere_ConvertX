using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface IApprovalService
{
    Task<PagedResult<ApprovalRequestDto>> GetQueueAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<ApprovalRequestDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SavePolicyAsync(int? id, SaveApprovalPolicyRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ApprovalPolicyDto>> GetPoliciesAsync(CancellationToken cancellationToken = default);
    Task<int> CreateRequestAsync(CreateApprovalRequestDto request, CancellationToken cancellationToken = default);
    Task ActAsync(int id, ApprovalActionRequestDto request, CancellationToken cancellationToken = default);
    Task CancelAsync(int id, string? note, CancellationToken cancellationToken = default);
}

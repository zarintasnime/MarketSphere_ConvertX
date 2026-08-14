using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.KPI.DTOs;

namespace MarketSphere.Application.Modules.KPI.Interfaces;

public interface IRewardService
{
    Task<PagedResult<RewardRuleDto>> GetRulesAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<int> CreateRuleAsync(SaveRewardRuleRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateRuleAsync(int id, SaveRewardRuleRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CalculateAsync(CalculateRewardRequestDto request, CancellationToken cancellationToken = default);
    Task<PagedResult<RewardCalculationDto>> GetCalculationsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task AdjustAsync(int id, AdjustRewardRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int id, ChangeRewardStatusRequestDto request, CancellationToken cancellationToken = default);
}

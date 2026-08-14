using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.CRM.DTOs;

namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface ILeadService
{
    Task<PagedResult<LeadListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<LeadDetailsDto> GetByIdAsync(int leadID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveLeadRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int leadID, SaveLeadRequestDto request, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(int leadID, ChangeLeadStatusRequestDto request, CancellationToken cancellationToken = default);
    Task<LeadScoreResultDto> RecalculateScoreAsync(int leadID, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DuplicateCandidateDto>> FindDuplicatesAsync(int leadID, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DuplicateReviewDto>> GetDuplicateReviewsAsync(CancellationToken cancellationToken = default);
    Task ResolveDuplicateReviewAsync(int duplicateReviewCaseID, ResolveDuplicateReviewRequestDto request, CancellationToken cancellationToken = default);
    Task<int> CreateScoreRuleAsync(SaveLeadScoreRuleRequestDto request, CancellationToken cancellationToken = default);
    Task<int> ConvertToClientAsync(int leadID, ConvertLeadToClientRequestDto request, CancellationToken cancellationToken = default);
}

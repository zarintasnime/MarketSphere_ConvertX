using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.MarketingField.DTOs;

namespace MarketSphere.Application.Modules.MarketingField.Interfaces;

public interface IFeedbackService
{
    Task<PagedResult<FeedbackListDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<FeedbackDetailsDto> GetByIdAsync(int feedbackID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveFeedbackRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int feedbackID, SaveFeedbackRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int feedbackID, CancellationToken cancellationToken = default);
}

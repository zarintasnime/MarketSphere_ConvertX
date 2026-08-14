using MarketSphere.Application.Modules.CRM.DTOs;
namespace MarketSphere.Application.Modules.CRM.Interfaces;
public interface ICrmActivityService
{
    Task<IReadOnlyCollection<CrmActivityDto>> GetTimelineAsync(int? leadID, int? clientID, int? opportunityID, CancellationToken cancellationToken = default);
    Task<CrmActivityDto> GetByIdAsync(int activityID, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SaveCrmActivityRequestDto request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int activityID, SaveCrmActivityRequestDto request, CancellationToken cancellationToken = default);
}

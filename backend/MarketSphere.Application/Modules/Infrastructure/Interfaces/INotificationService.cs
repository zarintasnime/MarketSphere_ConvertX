using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface INotificationService
{
    Task<PagedResult<NotificationDto>> GetMineAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateNotificationRequestDto request, CancellationToken cancellationToken = default);
    Task MarkReadAsync(int id, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(CancellationToken cancellationToken = default);
}

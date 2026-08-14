using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface ISettingService
{
    Task<IReadOnlyCollection<SystemSettingDto>> GetAsync(CancellationToken cancellationToken = default);
    Task<SystemSettingDto?> GetByKeyAsync(string key, int? scopeID = null, CancellationToken cancellationToken = default);
    Task<int> SaveAsync(int? id, SaveSystemSettingRequestDto request, CancellationToken cancellationToken = default);
}

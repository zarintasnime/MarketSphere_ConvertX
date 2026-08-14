using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface ILookupService
{
    Task<LookupGroupDto> GetAsync(string code, CancellationToken cancellationToken = default);
}

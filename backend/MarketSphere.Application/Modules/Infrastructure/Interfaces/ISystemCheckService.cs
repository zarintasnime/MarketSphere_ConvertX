using MarketSphere.Application.Modules.Infrastructure.DTOs;

namespace MarketSphere.Application.Modules.Infrastructure.Interfaces;

public interface ISystemCheckService
{
    Task<SystemCheckRunDto> RunAsync(CancellationToken cancellationToken = default);
}

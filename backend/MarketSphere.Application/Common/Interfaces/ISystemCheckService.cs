using MarketSphere.Application.Common.Models;

namespace MarketSphere.Application.Common.Interfaces;

public interface ISystemCheckService
{
    Task<IReadOnlyCollection<SystemCheckResult>> RunAsync(
        CancellationToken cancellationToken = default);
}

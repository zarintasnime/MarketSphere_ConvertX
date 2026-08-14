using MarketSphere.Application.Modules.Infrastructure.DTOs;
using MarketSphere.Application.Modules.Infrastructure.Interfaces;
using CommonSystemCheckService = MarketSphere.Application.Common.Interfaces.ISystemCheckService;
using DateTimeProvider = MarketSphere.Application.Common.Interfaces.IDateTimeProvider;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

public sealed class SystemCheckService : ISystemCheckService
{
    private readonly CommonSystemCheckService _systemChecks;
    private readonly DateTimeProvider _clock;

    public SystemCheckService(CommonSystemCheckService systemChecks, DateTimeProvider clock)
    {
        _systemChecks = systemChecks;
        _clock = clock;
    }

    public async Task<SystemCheckRunDto> RunAsync(CancellationToken cancellationToken = default)
    {
        var results = await _systemChecks.RunAsync(cancellationToken);
        var items = results.Select(x => new SystemCheckItemDto(x.CheckName, x.CheckName, x.MatchCount, $"{x.MatchCount} matching records were found.", null, null)).ToArray();
        return new SystemCheckRunDto(_clock.UtcNow, results.Sum(x => x.NotificationsCreated), items);
    }
}

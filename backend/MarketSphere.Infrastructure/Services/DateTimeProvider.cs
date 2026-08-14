using MarketSphere.Application.Common.Interfaces;

namespace MarketSphere.Infrastructure.Services;

public sealed class DateTimeProvider :
    IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly UtcToday =>
        DateOnly.FromDateTime(DateTime.UtcNow);
}

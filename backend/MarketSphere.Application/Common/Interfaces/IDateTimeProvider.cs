namespace MarketSphere.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly UtcToday { get; }
}

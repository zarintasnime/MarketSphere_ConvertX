namespace MarketSphere.Api.Contracts;

public sealed record HealthResponse(
    string Status,
    string Application,
    DateTime UtcTime);

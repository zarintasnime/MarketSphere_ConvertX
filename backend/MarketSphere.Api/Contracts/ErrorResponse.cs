namespace MarketSphere.Api.Contracts;

public sealed record ErrorResponse(
    bool Succeeded,
    string Message,
    string ErrorCode,
    string TraceID,
    IReadOnlyDictionary<string, string[]>? Errors = null);

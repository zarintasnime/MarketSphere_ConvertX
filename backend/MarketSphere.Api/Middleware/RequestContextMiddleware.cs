namespace MarketSphere.Api.Middleware;

public sealed class RequestContextMiddleware
{
    public const string CorrelationHeader =
        "X-Correlation-ID";

    private readonly RequestDelegate _next;

    public RequestContextMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        var correlationID =
            context.Request.Headers.TryGetValue(
                CorrelationHeader,
                out var suppliedValue) &&
            !string.IsNullOrWhiteSpace(
                suppliedValue.ToString())
                ? suppliedValue.ToString()
                : Guid.NewGuid().ToString("N");

        context.TraceIdentifier = correlationID;
        context.Items[CorrelationHeader] = correlationID;
        context.Response.Headers[CorrelationHeader] =
            correlationID;

        await _next(context);
    }
}

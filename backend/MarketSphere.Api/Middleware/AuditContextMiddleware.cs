namespace MarketSphere.Api.Middleware;

public sealed class AuditContextMiddleware
{
    public const string IpAddressItem = "Audit.IPAddress";
    public const string DeviceIdentifierItem =
        "Audit.DeviceIdentifier";

    private readonly RequestDelegate _next;

    public AuditContextMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        context.Items[IpAddressItem] =
            context.Connection.RemoteIpAddress?
                .ToString();

        context.Items[DeviceIdentifierItem] =
            context.Request.Headers[
                "X-Device-Identifier"]
                .FirstOrDefault();

        await _next(context);
    }
}

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;

namespace MarketSphere.Api.Middleware;

public sealed class IdempotencyMiddleware
{
    public const string IdempotencyHeader = "Idempotency-Key";
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IIdempotencyService idempotencyService)
    {
        if (!RequiresIdempotency(context.Request) || !context.Request.Headers.TryGetValue(IdempotencyHeader, out var keyValues) || string.IsNullOrWhiteSpace(keyValues.ToString()))
        {
            await _next(context);
            return;
        }

        var key = keyValues.ToString().Trim();
        if (key.Length > 200)
        {
            await WriteConflictAsync(context, "Idempotency key cannot exceed 200 characters.");
            return;
        }

        context.Request.EnableBuffering();
        string body;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, 8192, true)) body = await reader.ReadToEndAsync(context.RequestAborted);
        context.Request.Body.Position = 0;
        var endpoint = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{endpoint}\n{body}")));
        var userID = int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub"), out var parsedUserID) ? parsedUserID : (int?)null;
        var offline = BuildOfflineContext(context, body);
        var begin = await idempotencyService.BeginAsync(key, userID, endpoint, hash, offline, context.RequestAborted);

        if (begin.IsConflict)
        {
            await WriteConflictAsync(context, "The idempotency key is already in use with a different or unfinished request.");
            return;
        }
        if (begin.IsReplay)
        {
            context.Response.StatusCode = begin.ResponseStatusCode ?? StatusCodes.Status200OK;
            context.Response.ContentType = "application/json";
            if (!string.IsNullOrEmpty(begin.ResponseBody)) await context.Response.WriteAsync(begin.ResponseBody, context.RequestAborted);
            return;
        }

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;
        try
        {
            await _next(context);
            buffer.Position = 0;
            var responseBody = await new StreamReader(buffer, Encoding.UTF8, false, 8192, true).ReadToEndAsync(context.RequestAborted);
            buffer.Position = 0;
            var serverEntityID = TryReadEntityID(responseBody);
            await idempotencyService.CompleteAsync(begin.IdempotencyRequestID, context.Response.StatusCode, responseBody, begin.OfflineSyncRecordID, serverEntityID, context.RequestAborted);
            await buffer.CopyToAsync(originalBody, context.RequestAborted);
        }
        catch (Exception ex)
        {
            await idempotencyService.FailAsync(begin.IdempotencyRequestID, begin.OfflineSyncRecordID, ex.Message, context.RequestAborted);
            throw;
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool RequiresIdempotency(HttpRequest request)
        => (request.Method is "POST" or "PUT" or "PATCH" or "DELETE") && !(request.ContentType?.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase) ?? false);

    private static OfflineSyncContext? BuildOfflineContext(HttpContext context, string body)
    {
        if (!int.TryParse(context.Request.Headers["X-User-Session-ID"].ToString(), out var sessionID)) return null;
        var localKey = context.Request.Headers["X-Local-Record-Key"].ToString().Trim();
        var entityType = context.Request.Headers["X-Entity-Type"].ToString().Trim();
        if (string.IsNullOrWhiteSpace(localKey) || string.IsNullOrWhiteSpace(entityType)) return null;
        var operation = int.TryParse(context.Request.Headers["X-Operation-Type"], out var operationValue) ? operationValue : 1;
        var timestamp = DateTime.TryParse(context.Request.Headers["X-Client-Timestamp"], out var parsedTimestamp) ? parsedTimestamp.ToUniversalTime() : DateTime.UtcNow;
        return new OfflineSyncContext(sessionID, localKey, entityType.ToUpperInvariant(), operation, body, timestamp);
    }

    private static int? TryReadEntityID(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return null;
        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Number && root.TryGetInt32(out var direct)) return direct;
            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var name in new[] { "id", "data", "entityID", "entityId" })
                    if (root.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var value)) return value;
            }
        }
        catch (JsonException) { }
        return null;
    }

    private static async Task WriteConflictAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "idempotency_conflict", message }, context.RequestAborted);
    }
}

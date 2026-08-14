using MarketSphere.Application.Common.Interfaces;
using MarketSphere.Application.Common.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MarketSphere.Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "MarketSphere.Api";
    public string Audience { get; init; } = "MarketSphere.Clients";
    public string SigningKey { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 30;
    public int RefreshTokenDays { get; init; } = 7;
}

public sealed class JwtTokenService :
    IJwtTokenService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly JwtOptions _options;
    private readonly IDateTimeProvider _clock;

    public JwtTokenService(
        IOptions<JwtOptions> options,
        IDateTimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;

        if (Encoding.UTF8.GetByteCount(
                _options.SigningKey) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey must contain at least 32 bytes.");
        }
    }

    public TokenResult CreateToken(
        CurrentUserInfo user)
    {
        var issuedAt = _clock.UtcNow;
        var accessExpiresAt = issuedAt.AddMinutes(
            _options.AccessTokenMinutes);

        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object?>
        {
            ["iss"] = _options.Issuer,
            ["aud"] = _options.Audience,
            ["sub"] = user.UserID.ToString(),
            ["employee_id"] = user.EmployeeID?.ToString(),
            ["name"] = user.FullName,
            ["email"] = user.Email,
            ["role"] = user.RoleCodes,
            ["permission"] = user.PermissionCodes,
            ["jti"] = Guid.NewGuid().ToString("N"),
            ["iat"] = ToUnixTimeSeconds(issuedAt),
            ["nbf"] = ToUnixTimeSeconds(issuedAt),
            ["exp"] = ToUnixTimeSeconds(accessExpiresAt)
        };

        var encodedHeader = Base64UrlEncode(
            JsonSerializer.SerializeToUtf8Bytes(
                header,
                JsonOptions));

        var encodedPayload = Base64UrlEncode(
            JsonSerializer.SerializeToUtf8Bytes(
                payload,
                JsonOptions));

        var unsignedToken =
            $"{encodedHeader}.{encodedPayload}";

        using var hmac = new HMACSHA256(
            Encoding.UTF8.GetBytes(
                _options.SigningKey));

        var signature = Base64UrlEncode(
            hmac.ComputeHash(
                Encoding.ASCII.GetBytes(unsignedToken)));

        var accessToken =
            $"{unsignedToken}.{signature}";

        var refreshToken = Base64UrlEncode(
            RandomNumberGenerator.GetBytes(64));

        return new TokenResult(
            accessToken,
            accessExpiresAt,
            refreshToken,
            issuedAt.AddDays(
                _options.RefreshTokenDays));
    }

    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));
    }

    private static long ToUnixTimeSeconds(
        DateTime value) =>
        new DateTimeOffset(
            DateTime.SpecifyKind(
                value,
                DateTimeKind.Utc))
            .ToUnixTimeSeconds();

    private static string Base64UrlEncode(
        byte[] value) =>
        Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}

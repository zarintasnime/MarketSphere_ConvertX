namespace MarketSphere.Application.Common.Models;

public sealed record TokenResult(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);

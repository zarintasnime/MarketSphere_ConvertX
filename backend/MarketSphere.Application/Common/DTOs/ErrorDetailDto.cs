namespace MarketSphere.Application.Common.DTOs;

public sealed record ErrorDetailDto(string Code, string Message, string? Field = null);

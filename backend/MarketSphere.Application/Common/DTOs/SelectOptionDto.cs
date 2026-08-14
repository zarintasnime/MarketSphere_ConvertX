namespace MarketSphere.Application.Common.DTOs;

public sealed record SelectOptionDto(string Value, string Label, bool Disabled = false);

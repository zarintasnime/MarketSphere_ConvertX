namespace MarketSphere.Application.Modules.Infrastructure.DTOs;

public sealed record LookupItemDto(int Value, string Label, string? Group, bool IsActive);
public sealed record LookupGroupDto(string Code, IReadOnlyCollection<LookupItemDto> Items);

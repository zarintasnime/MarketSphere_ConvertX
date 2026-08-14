namespace MarketSphere.Application.Modules.Infrastructure.DTOs;

public sealed record SystemCheckItemDto(string CheckCode, string Title, int MatchCount, string Message, string? ReferenceType, int? ReferenceID);
public sealed record SystemCheckRunDto(DateTime RanAt, int NotificationsCreated, IReadOnlyCollection<SystemCheckItemDto> Results);

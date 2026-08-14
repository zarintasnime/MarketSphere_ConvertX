namespace MarketSphere.Application.Common.Models;

public sealed record SystemCheckResult(string CheckName, int MatchCount, int NotificationsCreated);

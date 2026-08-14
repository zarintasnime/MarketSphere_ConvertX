namespace MarketSphere.Application.Common.Models;

public sealed class PagedRequest
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
}

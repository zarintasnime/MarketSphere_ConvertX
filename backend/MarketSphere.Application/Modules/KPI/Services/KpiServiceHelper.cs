using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Models;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.KPI.Services;

internal static class KpiServiceHelper
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(IQueryable<T> query, PagedRequest request, CancellationToken cancellationToken)
    {
        var page = request.PageNumber < 1 ? 1 : request.PageNumber;
        var size = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return PagedResult<T>.Create(items, count, page, size);
    }

    public static async Task<T> RequireAsync<T>(IQueryable<T> query, string name, CancellationToken cancellationToken)
        where T : class
        => await query.SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException($"{name} was not found.");

    public static decimal Percent(decimal actual, decimal target)
        => target <= 0 ? 0 : Math.Round(actual / target * 100m, 2, MidpointRounding.AwayFromZero);
}

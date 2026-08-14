using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.ProductPricing.Services;

internal static class ProductPricingServiceHelper
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        IQueryable<T> query,
        PagedRequest request,
        CancellationToken cancellationToken)
    {
        PaginationValidator.Validate(request);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return PagedResult<T>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public static async Task<TEntity> RequireAsync<TEntity>(
        IQueryable<TEntity> query,
        string entityName,
        CancellationToken cancellationToken)
        where TEntity : class
        => await query.SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException($"{entityName} was not found.");

    public static bool PeriodsOverlap(
        DateOnly firstFrom,
        DateOnly? firstTo,
        DateOnly secondFrom,
        DateOnly? secondTo)
    {
        var firstEnd = firstTo ?? DateOnly.MaxValue;
        var secondEnd = secondTo ?? DateOnly.MaxValue;
        return firstFrom <= secondEnd && secondFrom <= firstEnd;
    }
}

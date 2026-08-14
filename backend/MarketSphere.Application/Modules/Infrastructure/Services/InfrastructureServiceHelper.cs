using Microsoft.EntityFrameworkCore;
using MarketSphere.Application.Common.Models;
using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Modules.Infrastructure.Services;

internal static class InfrastructureServiceHelper
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

    public static string Required(string? value, string name, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new BusinessRuleException($"{name} is required.");
        var result = value.Trim();
        if (result.Length > maximumLength) throw new BusinessRuleException($"{name} cannot exceed {maximumLength} characters.");
        return result;
    }
}

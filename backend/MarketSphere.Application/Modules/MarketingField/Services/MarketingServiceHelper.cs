using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.MarketingField.Services;

internal static class MarketingServiceHelper
{
    public static async Task<T> RequireAsync<T>(IQueryable<T> query, string name, CancellationToken cancellationToken)
        where T : class
        => await query.SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"{name} was not found.");

    public static async Task<PagedResult<T>> ToPagedAsync<T>(IQueryable<T> query, PagedRequest request, CancellationToken cancellationToken)
    {
        PaginationValidator.Validate(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
        return PagedResult<T>.Create(items, total, request.PageNumber, request.PageSize);
    }

    public static void ValidateGps(decimal? latitude, decimal? longitude)
    {
        if (latitude.HasValue) ValidationHelper.Require(latitude is >= -90 and <= 90, nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude.HasValue) ValidationHelper.Require(longitude is >= -180 and <= 180, nameof(longitude), "Longitude must be between -180 and 180.");
        ValidationHelper.Require(latitude.HasValue == longitude.HasValue, nameof(longitude), "Latitude and longitude must be supplied together.");
    }

    public static void ValidatePercentage(decimal value, string field)
        => ValidationHelper.Require(value is >= 0 and <= 100, field, $"{field} must be between 0 and 100.");

    public static void ValidateScore(decimal? value, string field)
    {
        if (value.HasValue) ValidatePercentage(value.Value, field);
    }
}

using MarketSphere.Application.Common.Models;
using MarketSphere.Application.Common.Validation;
using MarketSphere.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MarketSphere.Application.Modules.CRM.Services;

internal static class CrmServiceHelper
{
    public static async Task<T> RequireAsync<T>(IQueryable<T> query, string name, CancellationToken cancellationToken)
        where T : class
        => await query.SingleOrDefaultAsync(cancellationToken)
           ?? throw new NotFoundException($"{name} was not found.");

    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        IQueryable<T> query,
        PagedRequest request,
        CancellationToken cancellationToken)
    {
        PaginationValidator.Validate(request);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return PagedResult<T>.Create(items, total, request.PageNumber, request.PageSize);
    }

    public static void ValidatePositiveId(int value, string field)
        => ValidationHelper.Require(value > 0, field, $"{field} must be greater than zero.");

    public static void ValidateOptionalPositiveId(int? value, string field)
    {
        if (value.HasValue)
            ValidatePositiveId(value.Value, field);
    }

    public static void ValidateDateRange(DateOnly start, DateOnly end, string endField)
        => ValidationHelper.Require(end >= start, endField, $"{endField} must be on or after the start date.");

    public static void ValidateDateTimeRange(DateTime? start, DateTime? end, string endField)
    {
        if (start.HasValue && end.HasValue)
            ValidationHelper.Require(end >= start, endField, $"{endField} must be on or after the start time.");
    }

    public static void ValidateGps(decimal? latitude, decimal? longitude)
    {
        if (latitude.HasValue)
            ValidationHelper.Require(latitude is >= -90 and <= 90, nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude.HasValue)
            ValidationHelper.Require(longitude is >= -180 and <= 180, nameof(longitude), "Longitude must be between -180 and 180.");
    }

    public static string EscapeLike(string value) => value.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
}

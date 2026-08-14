namespace MarketSphere.Application.Common.Mapping;

public static class MappingExtensions
{
    public static string NormalizeCode(this string value) =>
        value.Trim().ToUpperInvariant();

    public static string NormalizeEmail(this string value) =>
        value.Trim().ToLowerInvariant();

    public static string? NullIfWhiteSpace(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

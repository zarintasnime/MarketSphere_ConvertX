using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Common.Validation;

public static class ValidationHelper
{
    public static void Require(
        bool condition,
        string field,
        string message)
    {
        if (!condition)
        {
            throw new AppValidationException(
                new Dictionary<string, string[]>
                {
                    [field] = new[] { message }
                });
        }
    }

    public static void RequireNotBlank(
        string? value,
        string field,
        int maxLength)
    {
        Require(
            !string.IsNullOrWhiteSpace(value),
            field,
            $"{field} is required.");

        Require(
            value!.Trim().Length <= maxLength,
            field,
            $"{field} cannot exceed {maxLength} characters.");
    }
}

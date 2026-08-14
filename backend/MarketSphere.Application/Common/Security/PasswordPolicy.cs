using MarketSphere.Domain.Exceptions;

namespace MarketSphere.Application.Common.Security;

public static class PasswordPolicy
{
    public const int MinimumLength = 8;

    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new AppValidationException(
                new Dictionary<string, string[]>
                {
                    ["Password"] =
                        new[] { "Password is required." }
                });
        }

        var errors = new List<string>();

        if (password.Length < MinimumLength)
        {
            errors.Add(
                $"Password must be at least {MinimumLength} characters long.");
        }

        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain an uppercase letter.");

        if (!password.Any(char.IsLower))
            errors.Add("Password must contain a lowercase letter.");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain a number.");

        if (!password.Any(
                character =>
                    !char.IsLetterOrDigit(character)))
        {
            errors.Add(
                "Password must contain a special character.");
        }

        if (errors.Count > 0)
        {
            throw new AppValidationException(
                new Dictionary<string, string[]>
                {
                    ["Password"] = errors.ToArray()
                });
        }
    }
}

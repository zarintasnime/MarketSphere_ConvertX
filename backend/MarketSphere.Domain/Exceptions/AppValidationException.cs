namespace MarketSphere.Domain.Exceptions;

public class AppValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public AppValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public AppValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}

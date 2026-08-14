namespace MarketSphere.Application.Common.Models;

public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Success") => new()
    {
        Succeeded = true,
        Message = message,
        Data = data
    };

    public static ApiResponse<T> Failure(
        string message,
        IReadOnlyDictionary<string, string[]>? errors = null) => new()
        {
            Succeeded = false,
            Message = message,
            Errors = errors
        };
}

namespace MarketSphere.Domain.Exceptions;

public class ForbiddenBusinessActionException : Exception
{
    public ForbiddenBusinessActionException(string message) : base(message) { }
}

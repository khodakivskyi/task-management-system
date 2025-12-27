namespace backend.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized") : base(message) { }
    public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
}

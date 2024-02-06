namespace JWT_Token.CustomExceptions;

public class RequestFailedException : Exception
{
    public RequestFailedException() : base() { }
    public RequestFailedException(string? msg) : base(msg) { }
}

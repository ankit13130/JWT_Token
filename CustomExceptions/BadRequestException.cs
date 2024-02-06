namespace JWT_Token.CustomExceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base() { }
    public BadRequestException(string? msg) : base(msg) { }
}

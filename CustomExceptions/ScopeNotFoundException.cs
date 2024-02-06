namespace JWT_Token.CustomExceptions;

public class ScopeNotFoundException : Exception
{
    public ScopeNotFoundException() : base() { }
    public ScopeNotFoundException(string? msg) : base(msg) { }
}

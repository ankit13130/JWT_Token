namespace JWT_Token.RequestModels;

public record UserRequestModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

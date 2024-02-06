using System.ComponentModel.DataAnnotations;

namespace JWT_Token.Models;

public class Users : Audit
{
    [Key]
    public long UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Hash { get; set; }
    public string Salt { get; set; }
    public string Roles { get; set; } = "USER";
}

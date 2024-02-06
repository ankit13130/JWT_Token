using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWT_Token.Controllers;

[Authorize(Roles = "ADMIN, USER")]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DataContext _dataContext;
    public UserController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserMessage()
    {
        return Ok($"Authorized By JWT & Login as {User.FindFirstValue(ClaimTypes.Role)} with Username {User.Identity.Name}");
    }
}
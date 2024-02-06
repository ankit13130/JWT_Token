using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWT_Token.Controllers;


//[Authorize(Roles = "ADMIN")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly DataContext _dataContext;
    public AdminController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAdminMessage()
    {
        var aa = User.FindAll(ClaimTypes.Role).Select(x =>x.Value).ToList();
        return Ok($"Authorized By JWT & Login as {User.FindFirstValue(ClaimTypes.Role)} with Username {User.Identity.Name}");
    }
}

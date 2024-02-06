using JWT_Token.EncryptDecrypt;
using JWT_Token.Models;
using JWT_Token.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Token.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _dataContext;
    public ValidationController(IConfiguration configuration, DataContext dataContext)
    {
        _configuration = configuration;
        _dataContext = dataContext;

    }
    private Users AuthenticateUser(UserRequestModel userRequestModel)
    {
        Users user = null;
        EncryptionDecryption encryptionDecryption = new EncryptionDecryption();
        var data = _dataContext.Users.Where(x => x.Username == userRequestModel.Username);

        if (!data.Any() || !data.FirstOrDefault().IsActive)
            throw new Exception("User Not Found");
        
        user = data.FirstOrDefault();
        if (!encryptionDecryption.VerifyPassword(userRequestModel.Password, user.Hash, Convert.FromHexString(user.Salt)))
            throw new Exception("Wrong Password");
        
        return user;
    }
    private string GenerateToken(Users users)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //without claims
        //var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
        //    _configuration["Jwt:Audience"],
        //    null,
        //    expires: DateTime.Now.AddMinutes(20),
        //    signingCredentials: credentials);
        //return new JwtSecurityTokenHandler().WriteToken(token);

        //with claims
        //var tokenDescriptor = new SecurityTokenDescriptor()
        //{
        //    Subject = new ClaimsIdentity(new[]
        //    {
        //        new Claim("UserId",users.UserId.ToString()),
        //        new Claim("Username",users.Username),
        //        new Claim("Roles",users.Roles),                        //not using because it gives error in role based authentication
        //    }),
        //    Expires = DateTime.UtcNow.AddSeconds(10),
        //    Issuer = _configuration["Jwt:Issuer"],
        //    Audience = _configuration["Jwt:Audience"],
        //    SigningCredentials = credentials,
        //};
        //var tokenHandler = new JwtSecurityTokenHandler();
        //var token = tokenHandler.CreateToken(tokenDescriptor);
        //return tokenHandler.WriteToken(token);

        //second method with claims
        var claims = new[]
        {
            new Claim(ClaimTypes.Sid,users.UserId.ToString()),
            new Claim(ClaimTypes.Name,users.Username),
            new Claim(ClaimTypes.Role,"ADMIN"),
            new Claim(ClaimTypes.Role,"USER")
        };

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //[AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserRequestModel userRequestModel)
    {
        IActionResult response = Unauthorized();
        Users user = AuthenticateUser(userRequestModel);
        if (user != null)
        {
            var tokenString = GenerateToken(user);
            response = Ok(tokenString);
        }

        return response;
    }

    //[AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] UserRequestModel userRequestModel)
    {
        if (_dataContext.Users.Any(x => x.Username == userRequestModel.Username && x.Password == userRequestModel.Password))
        {
            throw new Exception("User Already Exists");
        }

        Users users = new Users();
        users.Username = userRequestModel.Username;
        users.Password = userRequestModel.Password;

        EncryptionDecryption encryptDecrypt = new EncryptionDecryption();
        
        users.Hash = encryptDecrypt.HashPasword(userRequestModel.Password, out var salt);
        users.Salt = Convert.ToHexString(salt);

        _dataContext.Users.Add(users);
        await _dataContext.SaveChangesAsync();
        return Ok($"{userRequestModel.Username} Added Successfully");
    }
}

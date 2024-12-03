using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiArariwa.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    
    private IConfiguration _config;
    
    public LoginController(IConfiguration config)
    {
        _config = config;
    }

    private User AuthenticateUser(User userLogin)
    {
        User _user = null;

        if (userLogin.Username == "admin" && userLogin.Password == "admin")
        {
            _user = new User { Username = "admin", Password = "admin" };
        }
        
        return _user;
    }

    private string GenerateToke(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            null,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(User user)
    {
        IActionResult response = Unauthorized();
        var _user = AuthenticateUser(user);
        if (_user != null)
        {
            var tokenString = GenerateToke(_user);
            response = Ok(new { token = tokenString });
        }
        return response;
    }
}
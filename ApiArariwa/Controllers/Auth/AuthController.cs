using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiArariwa.Auditoria;
using ApiArariwa.Dapper;
using ApiArariwa.Dapper.Auditoria;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiArariwa.Controllers;

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IAuditoriaService _auditoria;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IAuditoriaService auditoria)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _auditoria     = auditoria;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("Usuario registrado exitosamente.");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        
        var userLog = new UserLoginLogs
        {
            UserId       = user.Id,
            AttemptDate  = DateTime.Now,
            IpAddress    = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
        };
        
        if (user == null)
        {
            userLog.IsSuccessful = false;
            await _auditoria.SetUserLoginLogsAsync(userLog);
            return Unauthorized("Usuario o contraseña incorrectos.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        
        if (!result.Succeeded)
        {
            userLog.IsSuccessful = false;
            await _auditoria.SetUserLoginLogsAsync(userLog);
            return Unauthorized("Usuario o contraseña incorrectos.");
        }
        
        userLog.IsSuccessful = true;
        await _auditoria.SetUserLoginLogsAsync(userLog);
        
        var token = await GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
        };
        
        // Agrega los roles como claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer            : _configuration["Jwt:Issuer"],
            audience          : _configuration["Jwt:Audience"],
            claims            : claims,
            expires           : DateTime.Now.AddMinutes(720),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

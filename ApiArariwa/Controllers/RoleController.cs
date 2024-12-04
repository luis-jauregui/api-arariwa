using System.Security.Claims;
using ApiArariwa.Dapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ApiArariwa.Controllers;

public class AddClaimToRoleRequest
{
    public string RoleName { get; set; }
    public int IdClaim { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly GenericRepository _repository;

    public RolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, GenericRepository repository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _repository = repository;
    }
    
    [HttpGet("user-roles")]
    public async Task<IActionResult> GetUserRoles([FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }
    
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { Email = email, Roles = roles });
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (result.Succeeded)
        {
            return Ok($"El rol '{request.Role}' fue asignado al usuario '{request.Email}'.");
        }

        return BadRequest($"Error al asignar el rol: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return BadRequest("El nombre del rol no puede estar vacío.");
        }

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (roleExists)
        {
            return BadRequest("El rol ya existe.");
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded)
        {
            return Ok($"Rol '{roleName}' creado exitosamente.");
        }

        return StatusCode(500, "Ocurrió un error al crear el rol.");
    }
    
    [HttpGet("check-role")]
    public async Task<IActionResult> CheckUserRole([FromQuery] string email, [FromQuery] string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"Usuario con correo {email} no encontrado.");
        }
    
        var isInRole = await _userManager.IsInRoleAsync(user, role);
        if (isInRole)
        {
            return Ok($"El usuario {email} ya tiene el rol {role} asignado.");
        }
    
        return Ok($"El usuario {email} no tiene el rol {role} asignado.");
    }
    
    [HttpPost("add-claim-to-role")]
    public async Task<IActionResult> AddClaimToRole([FromBody] AddClaimToRoleRequest request)
    {
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
        {
            return NotFound($"El rol '{request.RoleName}' no existe.");
        }
        
        DynamicParameters parameters = new DynamicParameters();
        
        parameters.Add("@IdRole", role.Id);
        parameters.Add("@IdClaim", request.IdClaim);
        
        await _repository.Insert("sp_ins_RoleClaims", parameters);
        return Ok("Claim agregado al rol");
    
    }
    
    [HttpGet("get-role-claims")]
    public async Task<IActionResult> GetRoleClaims([FromQuery] string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return NotFound($"El rol '{roleName}' no existe.");
        }
    
        var claims = await _roleManager.GetClaimsAsync(role);
        return Ok(new { Role = roleName, Claims = claims.Select(c => new { c.Type, c.Value }) });
    }
    
}
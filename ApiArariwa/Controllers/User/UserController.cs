using ApiArariwa.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace ApiArariwa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("list-users")]
    public async Task<IActionResult> ListUsers()
    {
        return Ok(await _service.GetUsers());
    }

}

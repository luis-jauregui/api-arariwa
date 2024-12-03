using Microsoft.AspNetCore.Identity;

namespace ApiArariwa;

public class CustomUser: IdentityUser
{
    public string Username { get; set; }
    public string Password { get; set; }
}
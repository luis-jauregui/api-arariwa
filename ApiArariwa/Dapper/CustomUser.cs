using Microsoft.AspNetCore.Identity;

namespace ApiArariwa;

public class CustomUser: IdentityUser
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AssignRoleRequest
{
    public string Email { get; set; }
    public string Role { get; set; }
}

public class ClaimData
{
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
}

public class DtoGetUsers
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}
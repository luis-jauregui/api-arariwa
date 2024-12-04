using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;

public class UserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>, IUserEmailStore<IdentityUser>, IUserRoleStore<IdentityUser>
{
    private readonly IDbConnection _dbConnection;

    public UserStore(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
    {
        // Asigna el nombre de usuario al objeto usuario proporcionado.
        user.UserName = userName;

        // Devuelve una tarea completada porque no hay lógica adicional.
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        // Devuelve el nombre de usuario normalizado del usuario.
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
    {
        // Establece el nombre de usuario normalizado.
        user.NormalizedUserName = normalizedName;

        // Devuelve una tarea completada.
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string query = @"
            INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash)
            VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @PasswordHash)";
        
        var result = await _dbConnection.ExecuteAsync(query, new
        {
            user.Id,
            user.UserName,
            user.NormalizedUserName,
            user.Email,
            user.NormalizedEmail,
            user.PasswordHash
        });

        return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string query = @"
                UPDATE AspNetUsers
                SET UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    Email = @Email,
                    NormalizedEmail = @NormalizedEmail,
                    PasswordHash = @PasswordHash
                WHERE Id = @Id";

        var result = await _dbConnection.ExecuteAsync(query, new
        {
            user.UserName,
            user.NormalizedUserName,
            user.Email,
            user.NormalizedEmail,
            user.PasswordHash,
            user.Id
        });

        return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string query = "DELETE FROM AspNetUsers WHERE Id = @Id";

        var result = await _dbConnection.ExecuteAsync(query, new { user.Id });

        return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM AspNetUsers WHERE Id = @Id";
        return await _dbConnection.QueryFirstOrDefaultAsync<IdentityUser>(query, new { Id = userId });
    }

    public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";
        return await _dbConnection.QueryFirstOrDefaultAsync<IdentityUser>(query, new { NormalizedUserName = normalizedUserName });
    }

    // Implementa otros métodos requeridos por IUserStore e IUserPasswordStore.
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
    {
        // Establece el hash de la contraseña en el usuario.
        user.PasswordHash = passwordHash;

        // Devuelve una tarea completada porque no hay lógica adicional.
        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
    }

    public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
    {
        // Establece el correo electrónico del usuario.
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        // Devuelve el correo electrónico del usuario.
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        // Devuelve si el correo electrónico está confirmado.
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
    {
        // Establece si el correo electrónico está confirmado.
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        // Busca un usuario por su correo electrónico normalizado en la base de datos.
        const string query = "SELECT * FROM AspNetUsers WHERE NormalizedEmail = @NormalizedEmail";
        return _dbConnection.QueryFirstOrDefaultAsync<IdentityUser>(query, new { NormalizedEmail = normalizedEmail });
    }

    public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        // Devuelve el correo electrónico normalizado del usuario.
        return Task.FromResult(user.NormalizedEmail);
    }

    public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
        // Establece el correo electrónico normalizado del usuario.
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        const string roleQuery = "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedRoleName";
        var roleId = await _dbConnection.ExecuteScalarAsync<string>(roleQuery, new { NormalizedRoleName = roleName });

        if (roleId == null)
        {
            throw new InvalidOperationException($"El rol '{roleName}' no existe.");
        }

        const string userRoleQuery = "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
        await _dbConnection.ExecuteAsync(userRoleQuery, new { UserId = user.Id, RoleId = roleId });
    }

    public async Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        const string roleQuery = "SELECT Id FROM AspNetRoles WHERE NormalizedName = @NormalizedRoleName";
        var roleId = await _dbConnection.ExecuteScalarAsync<string>(roleQuery, new { NormalizedRoleName = roleName });

        if (roleId == null)
        {
            throw new InvalidOperationException($"El rol '{roleName}' no existe.");
        }

        const string userRoleQuery = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
        await _dbConnection.ExecuteAsync(userRoleQuery, new { UserId = user.Id, RoleId = roleId });
    }

    public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT r.Name
            FROM AspNetRoles r
            INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId";

        var roles = await _dbConnection.QueryAsync<string>(query, new { UserId = user.Id });
        return roles.ToList();
    }

    public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT COUNT(*)
            FROM AspNetRoles r
            INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId AND r.NormalizedName = @NormalizedRoleName";

        var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { UserId = user.Id, NormalizedRoleName = roleName });
        return count > 0;
    }

    public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT u.*
            FROM AspNetUsers u
            INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE r.NormalizedName = @NormalizedRoleName";

        var users = await _dbConnection.QueryAsync<IdentityUser>(query, new { NormalizedRoleName = roleName });
        return users.ToList();
    }
}

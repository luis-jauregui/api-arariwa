using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;

public class UserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>
{
    private readonly IDbConnection _dbConnection;

    public UserStore(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string query = @"
            INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, SecurityStamp, ConcurrencyStamp)
            VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @PasswordHash, @SecurityStamp, @ConcurrencyStamp)";

        var result = await _dbConnection.ExecuteAsync(query, new
        {
            user.Id,
            user.UserName,
            user.NormalizedUserName,
            user.Email,
            user.NormalizedEmail,
            user.PasswordHash,
            user.SecurityStamp,
            user.ConcurrencyStamp
        });

        return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        const string query = @"
            UPDATE AspNetUsers
            SET UserName = @UserName, NormalizedUserName = @NormalizedUserName, Email = @Email, 
                NormalizedEmail = @NormalizedEmail, PasswordHash = @PasswordHash, 
                SecurityStamp = @SecurityStamp, ConcurrencyStamp = @ConcurrencyStamp
            WHERE Id = @Id";

        var result = await _dbConnection.ExecuteAsync(query, user);
        return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM AspNetUsers WHERE Id = @UserId";
        return await _dbConnection.QueryFirstOrDefaultAsync<IdentityUser>(query, new { UserId = userId });
    }

    public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";
        return await _dbConnection.QueryFirstOrDefaultAsync<IdentityUser>(query, new { NormalizedUserName = normalizedUserName });
    }

    public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public void Dispose()
    {
        // No necesitas liberar recursos porque Dapper usa conexiones administradas.
    }

    // Implementa otros métodos requeridos por IUserStore e IUserPasswordStore según tus necesidades.
}

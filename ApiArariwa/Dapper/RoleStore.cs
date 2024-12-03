using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace ApiArariwa.Controllers;

public class RoleStore : IRoleStore<IdentityRole>
{
    private readonly IDbConnection _dbConnection;

    public RoleStore(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        const string query = @"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
        
        var result = await _dbConnection.ExecuteAsync(query, role);
        return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        const string query = "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName";
        return await _dbConnection.QueryFirstOrDefaultAsync<IdentityRole>(query, new { NormalizedName = normalizedRoleName });
    }

    public void Dispose()
    {
        // No necesitas liberar recursos porque Dapper usa conexiones administradas.
    }

    // Implementa otros métodos requeridos por IRoleStore según tus necesidades.
}

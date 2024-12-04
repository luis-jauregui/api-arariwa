using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiArariwa.Dapper;

public class GenericRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public GenericRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<T>> List<T>(string procedureName, DynamicParameters parameters = null)
    {

        if (parameters is null)
        {
            parameters = new DynamicParameters();
        }
        
        try
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                IEnumerable<T> val = await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return val;
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error ejecutando el procedimiento {procedureName}: {ex.Message}", ex);
        }
    }
    
    public async Task<T> Get<T>(string procedureName, DynamicParameters parameters)
    {
        try
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var val = await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return (T)val;
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error ejecutando el procedimiento {procedureName}: {ex.Message}", ex);
        }
    }
    
    public async Task<object> Insert(string procedureName, DynamicParameters parameters = null)
    {

        if (parameters is null)
        {
            parameters = new DynamicParameters();
        }
        
        try
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                object val = await connection.QueryAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return val;
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error ejecutando el procedimiento {procedureName}: {ex.Message}", ex);
        }
    }
    
    public async Task<object> Update(string procedureName, DynamicParameters parameters)
    {
        try
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                object val = await connection.QueryAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return val;
            }
        }
        catch (SqlException ex)
        {
            throw new Exception($"Error ejecutando el procedimiento {procedureName}: {ex.Message}", ex);
        }
    }
    
}

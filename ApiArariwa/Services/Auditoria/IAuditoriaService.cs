using ApiArariwa.Auditoria;
using Microsoft.AspNetCore.Mvc;

namespace ApiArariwa.Dapper.Auditoria;

public interface IAuditoriaService
{
    public Task<string> SetUserLoginLogsAsync(UserLoginLogs userLoginLogs);
}
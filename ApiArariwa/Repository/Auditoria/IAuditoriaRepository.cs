using ApiArariwa.Auditoria;
using Microsoft.AspNetCore.Mvc;

namespace ApiArariwa.Dapper.Auditoria;

public interface IAuditoriaRepository
{
    public Task<string> SetUserLoginLogsAsync(UserLoginLogs userLoginLogs);
}
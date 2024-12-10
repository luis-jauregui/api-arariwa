using ApiArariwa.Auditoria;
using Dapper;

namespace ApiArariwa.Dapper.Auditoria;

public class AuditoriaService: IAuditoriaService
{
    
    private readonly DapperRepository _repository;
    
    public AuditoriaService (DapperRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<string> SetUserLoginLogsAsync(UserLoginLogs userLoginLogs)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserId", userLoginLogs.UserId);
        parameters.Add("@IsSuccessful", userLoginLogs.IsSuccessful);
        parameters.Add("@AttemptDate", userLoginLogs.AttemptDate);
        parameters.Add("@IpAddress", userLoginLogs.IpAddress);
        parameters.Add("@Details", userLoginLogs.Details);
        await _repository.Insert("sp_ins_UserLoginLogs", parameters);
        return "Ok";
    }
    
}
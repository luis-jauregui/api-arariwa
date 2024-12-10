using ApiArariwa.Dapper;
using Dapper;

namespace ApiArariwa.Services.User;

public class UserService: IUserService
{
    
    private readonly DapperRepository _repository;
    
    public UserService (DapperRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<DtoGetUsers>> GetUsers()
    {
        return (await _repository.List<DtoGetUsers>("sp_get_Users")).ToList();
    }
}
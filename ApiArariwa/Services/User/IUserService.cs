namespace ApiArariwa.Services.User;

public interface IUserService
{
    public Task<List<DtoGetUsers>> GetUsers();
}
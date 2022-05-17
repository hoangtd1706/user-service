namespace Ecoba.BaseService.Services.UserService;

public interface IUserService
{
    Task<IEnumerable<UserRecord>> GetAllAsync();
}
using Ecoba.Consul.ServiceQuery;
namespace Ecoba.BaseService.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IConsulService _consulService;
        public UserService(IConsulService consulService)
        {
            _consulService = consulService;
        }
        public async Task<IEnumerable<UserRecord>> GetAllAsync()
        {
            var result = await _consulService.GetAsync<IEnumerable<UserRecord>>("user-service", $"api/v1/users", true);
            if (result != null) return result;
            return null;
        }
    }
}
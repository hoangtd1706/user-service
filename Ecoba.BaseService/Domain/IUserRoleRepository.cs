using Ecoba.BaseService.Infrastructure;

namespace Ecoba.BaseService.Domain;

public interface IUserRoleRepository<TContext> where TContext : BaseDbContext
{
    public string MOD_ROLE { get; }
    Task<UserRole> GetAsync(string userNumber, string role);
    Task<UserRole> AddAsync(string userNumber, string role);
    Task<UserRole> RemoveAsync(string userNumber, string role);
}
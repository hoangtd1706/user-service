using Ecoba.BaseService.Domain;
using Ecoba.BaseService.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace Ecoba.BaseService.Infrastructure.Repositories;

public class UserRoleRepository<TContext> : IUserRoleRepository<TContext> where TContext : BaseDbContext
{
    private readonly TContext _context;
    private readonly IUserService _userSer;
    private readonly IRoleConfig _roleConfig;

    public string MOD_ROLE { get => "MOD_ROLE"; }

    public UserRoleRepository(TContext context, IUserService userSer, IRoleConfig roleConfig)
    {
        _context = context;
        _userSer = userSer;
        _roleConfig = roleConfig;
    }

    public async Task<UserRole> GetAsync(string userNumber, string role)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserNumber == userNumber && x.Role == role);
        if (userRole == null)
        {
            userRole = _context.UserRoles.Local.FirstOrDefault(x => x.UserNumber == userNumber && x.Role == role);
        }

        return userRole;
    }

    public async Task<UserRole> AddAsync(string userNumber, string role)
    {
        var users = await _userSer.GetAllAsync();
        var user = users.FirstOrDefault(x => x.EmployeeId == userNumber);
        if (user == null)
            throw new BaseServiceExceptions("User does not exist!");

        var roles = await _roleConfig.GetAllAsync();
        if (role != MOD_ROLE && !roles.Select(x => x.ToUpper()).Contains(role.ToUpper()))
            throw new BaseServiceExceptions("Role does not exist!");

        var exist = await GetAsync(userNumber, role);
        if (exist != null)
            throw new BaseServiceExceptions("User role is exist already!");

        var userRole = new UserRole(userNumber, role);
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return userRole;
    }

    public async Task<UserRole> RemoveAsync(string userNumber, string role)
    {
        var userRole = await GetAsync(userNumber, role);
        if (userRole == null)
            throw new BaseServiceExceptions("User role does not exist!");

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return userRole;
    }
}

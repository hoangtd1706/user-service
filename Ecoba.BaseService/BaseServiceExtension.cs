using Ecoba.BaseService.Domain;
using Ecoba.BaseService.Infrastructure.Repositories;
using Ecoba.BaseService.Services.UserService;
using Microsoft.Extensions.DependencyInjection;

namespace Ecoba.BaseService;

public static class BaseServiceExtension
{
    public static void AddBaseService(this IServiceCollection services, IRoleConfig roleConfig)
    {
        services.AddScoped(typeof(IUserRoleRepository<>), typeof(UserRoleRepository<>));
        services.AddSingleton(roleConfig);
        services.AddScoped<IUserService, UserService>();
    }
}
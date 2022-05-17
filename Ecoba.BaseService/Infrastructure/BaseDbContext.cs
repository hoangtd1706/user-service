using Ecoba.BaseService.Domain;
using Ecoba.ProjectSystem.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Ecoba.BaseService.Infrastructure;

public abstract class BaseDbContext : DbContext
{
    public const string BASE_DEFAULT_SCHEMA = "base_service";
    public BaseDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
    }
}

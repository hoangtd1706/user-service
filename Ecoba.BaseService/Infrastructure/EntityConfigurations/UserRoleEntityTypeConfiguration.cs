using Ecoba.BaseService.Domain;
using Ecoba.BaseService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecoba.ProjectSystem.Infrastructure.EntityConfigurations;

public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> partActivityConfiguration)
    {
        partActivityConfiguration.ToTable("user_roles", BaseDbContext.BASE_DEFAULT_SCHEMA);

        partActivityConfiguration.HasKey(x => new { x.UserNumber, x.Role });
        partActivityConfiguration.HasIndex(x => new { x.UserNumber, x.Role });

        partActivityConfiguration.Property(c => c.UserNumber).HasColumnName("user_number");
        partActivityConfiguration.Property(c => c.Role).HasColumnName("role");
    }
}

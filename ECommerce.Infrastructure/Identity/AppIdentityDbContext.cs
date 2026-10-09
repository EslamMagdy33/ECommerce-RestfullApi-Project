using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Identity;

public sealed class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) 
    : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>(options)
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppIdentityDbContext).Assembly,
            type => type.Namespace == "ECommerce.Infrastructure.Identity");
    }
}

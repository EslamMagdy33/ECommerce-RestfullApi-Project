using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Data.Interceptor;

public sealed class AuditInterceptor : IAuditInterceptor
{
    public void Apply(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTimeOffset.UtcNow;
                    break;
            }
        }
    }
}

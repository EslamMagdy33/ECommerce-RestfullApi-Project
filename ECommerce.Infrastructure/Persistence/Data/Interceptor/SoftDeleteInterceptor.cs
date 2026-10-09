using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Data.Interceptor;

public sealed class SoftDeleteInterceptor(IHardDeleteTracker hardDeleteTracker) : ISoftDeleteInterceptor
{
    public void Apply(DbContext dbContext)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<BaseEntity>())
        {
            if(entry.State != EntityState.Deleted)
                continue;

            if (hardDeleteTracker.IsTracked(entry.Entity))
                continue;

            entry.State = EntityState.Modified;
            entry.Entity.MarkAsDeleted();
        }
    }
}

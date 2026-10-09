using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Data.Interceptor;

public interface IAuditInterceptor
{
    void Apply(DbContext dbContext);
}

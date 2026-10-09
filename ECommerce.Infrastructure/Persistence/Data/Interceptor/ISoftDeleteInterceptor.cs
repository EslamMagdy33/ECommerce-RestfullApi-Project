using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Data.Interceptor;

public interface ISoftDeleteInterceptor
{
    void Apply(DbContext dbContext);
}

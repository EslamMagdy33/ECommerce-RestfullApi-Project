using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.persistence.Data.DbContexts;
using ECommerce.Infrastructure.Persistence.Data.Interceptor;
using System.Collections.Concurrent;

namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork(StoreDbContext dbContext,IAuditInterceptor auditInterceptor
    ,ISoftDeleteInterceptor softDeleteInterceptor, IHardDeleteTracker hardDeleteTracker) : IUnitOfWork
{
    // Type(Product)=>IRepo<Product>
    private readonly ConcurrentDictionary<Type, object> _Repos = new();

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
       var type= typeof(T);
        if (_Repos.TryGetValue(type, out var rebo))
            return (IRepository<T>) rebo;
        var newRepo = new Repository<T>(dbContext, hardDeleteTracker);
        _Repos.TryAdd(type, newRepo);
        return newRepo;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        softDeleteInterceptor.Apply(dbContext);
        auditInterceptor.Apply(dbContext);
        try
        {
            return await dbContext.SaveChangesAsync(ct);
        }
        finally
        {
            hardDeleteTracker.Clear();
        }
        
    }
}

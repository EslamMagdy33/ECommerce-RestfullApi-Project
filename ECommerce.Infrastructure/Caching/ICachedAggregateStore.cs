namespace ECommerce.Infrastructure.Caching;

public interface ICachedAggregateStore<T> where T : class
{
    Task<T?> GetAsync(string cacheKey,CancellationToken ct=default);
    Task<T> GetOrCreateAsync(string cacheKey, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default);

    Task SetAsync(string cacheKey, T value, CancellationToken ct=default);
    Task RemoveAsync(string cacheKey, CancellationToken ct=default);
}

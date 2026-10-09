using CloudinaryDotNet;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;

namespace ECommerce.Infrastructure.Caching;

public sealed class HybridBasketStore(ICachedAggregateStore<Basket> store) : IBasketStore
{
    public Task<Basket?> GetAsync(Guid BuyerId, CancellationToken ct = default) 
        => store.GetAsync(BuildCacheKey(BuyerId), ct);
    public Task<Basket> GetOrCreateAsync(Guid BuyerId, CancellationToken ct = default)
        => store.GetOrCreateAsync(BuildCacheKey(BuyerId), async _ =>
        {
            var createResult = Basket.CreateEmpty(BuyerId);
            if (createResult.IsFailure)
                throw new InvalidOperationException(createResult.Error.Messege);
            return createResult.Value!;
        },ct)!;

    public Task SaveAsync(Basket basket, CancellationToken ct = default)
        => store.SetAsync(BuildCacheKey(basket.BuyerId), basket, ct);
    public async Task<Result> DeleteAsync(Guid BuyerId, CancellationToken ct = default)
    {
        try
        {
            await store.RemoveAsync(BuildCacheKey(BuyerId), ct);
            return Result.Success();
        }
        catch (Exception)
        {
            return BasketErrors.DeleteFailed;
        }
    }
    private static string BuildCacheKey(Guid buyerId) => $"basket:{buyerId}";
}

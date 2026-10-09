using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Repositories;

public interface IBasketStore
{
    Task<Basket?> GetAsync(Guid BuyerId,CancellationToken ct=default);
    Task<Basket> GetOrCreateAsync(Guid BuyerId, CancellationToken ct = default);
    Task SaveAsync(Basket basket, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid BuyerId, CancellationToken ct = default);
}

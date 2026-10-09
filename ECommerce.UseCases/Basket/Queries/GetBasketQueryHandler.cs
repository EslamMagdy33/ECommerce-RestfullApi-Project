using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Queries;

public sealed class GetBasketQueryHandler(IBasketStore basketStore) : IRequestHandle<GetBasketQuery, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(GetBasketQuery request, CancellationToken ct = default)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, ct);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}

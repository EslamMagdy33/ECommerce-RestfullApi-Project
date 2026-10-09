using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.ClearBasket;

public sealed class ClearBasketCommandHandler(IBasketStore basketStore)
    : IRequestHandle<ClearBasketCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(ClearBasketCommand request, CancellationToken ct = default)
    {
        var basket=await basketStore.GetOrCreateAsync(request.BuyerId, ct);
        basket.Clear();
        await basketStore.SaveAsync(basket,ct);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}

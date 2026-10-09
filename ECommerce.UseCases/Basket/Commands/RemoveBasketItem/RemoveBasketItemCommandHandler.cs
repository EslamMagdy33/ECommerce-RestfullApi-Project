using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.RemoveBasketItem;

public sealed class RemoveBasketItemCommandHandler(IBasketStore basketStore)
    : IRequestHandle<RemoveBasketItemCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(RemoveBasketItemCommand request, CancellationToken ct = default)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, ct);

        var removeItem = basket.RemoveItem(request.ProductId);
        if (removeItem.IsFailure)
            return Result<GetBasketResponse>.Failure(removeItem.Error);

        await basketStore.SaveAsync(basket,ct);

        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}

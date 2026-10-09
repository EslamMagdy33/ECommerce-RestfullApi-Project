using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.MergeBasket;

public sealed class MergeBasketCommandHandler(IBasketStore basketStore)
    : IRequestHandle<MergeBasketCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(MergeBasketCommand request, CancellationToken ct = default)
    {
        var basket=await basketStore.GetOrCreateAsync(request.BuyerId, ct);

        if (request.AnonymousBuyerId == Guid.Empty)
            return Result<GetBasketResponse>.Failure(BasketErrors.AnonymousBuyerRequired);

        var anonymousBasket=await basketStore.GetAsync(request.AnonymousBuyerId, ct);
        if (anonymousBasket is null || anonymousBasket.Items.Count == 0)
            return Result<GetBasketResponse>.Failure(BasketErrors.AnonymousBasketNotFound);

        var mergeBasket = basket.MergeFrom(anonymousBasket);
        if (mergeBasket.IsFailure)
            return Result<GetBasketResponse>.Failure(mergeBasket.Error);

        await basketStore.SaveAsync(basket,ct);
        await basketStore.DeleteAsync(anonymousBasket.BuyerId,ct);

        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}

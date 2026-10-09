using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandHandler(IBasketStore basketStore)
    : IRequestHandle<UpdateItemQuantityCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(UpdateItemQuantityCommand request, CancellationToken ct = default)
    {
        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, ct);

        var updateItem= basket.UpdateItemQuantity(request.ProductId, request.Quantity);
        if (updateItem.IsFailure)
            return Result<GetBasketResponse>.Failure(updateItem.Error);

        await basketStore.SaveAsync(basket, ct);

        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}

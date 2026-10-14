using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Basket.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Basket.Commands.AddBasketItem;

public sealed class AddItemToBasketCommandHandler(IBasketStore basketStore,IReadRepository<Product> productRepository)
    : IRequestHandle<AddItemToBasketCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(AddItemToBasketCommand request, CancellationToken ct = default)
    {
        var product = await productRepository.FirstOrDefaultAsync( 
            new ProductForBasketSpecification([request.ProductId]), ct);
        if (product is null)
            return Result<GetBasketResponse>.Failure(ProductErrors.NotFound);

        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, ct);

        var addResult = basket.AddItem(product.Id, product.Name, product.PictureUrl, product.Price, request.Quantity);
        if (addResult.IsFailure)
            return Result<GetBasketResponse>.Failure(addResult.Error);

        await basketStore.SaveAsync(basket, ct);

        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));

    }
}

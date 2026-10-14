using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;
using ECommerce.UseCases.Orders.Specifications;
using ECommerce.UseCases.Products.Specifications;
using Microsoft.Extensions.Logging;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    (ICurrentUserService currentUser,IBasketStore basketStore,IReadRepository<Product> productRepository,
    IReadRepository<UserAddress> addressRepository,
    IRepository<DeliveryMethod> deliveryMethodRepository,IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandle<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken ct = default)
    {
        if (currentUser.UserId is null)
            return Result<OrderResponse>.Failure(OrderErrors.Unauthorized);
        var userId = currentUser.UserId.Value;

        var address=await addressRepository.FirstOrDefaultAsync(
            new UserAddressByIdForUserSpecification(request.ShippingAddressId, userId), ct);
        if (address == null)
            return Result<OrderResponse>.Failure(OrderErrors.ShippingAddressNotFound);

        var deliveryMethod = await deliveryMethodRepository.GetByIdAsync(request.DeliverMethodId, ct);
        if(deliveryMethod == null)
            return Result<OrderResponse>.Failure(DeliveryMethodErrors.NotFound);

        var basket = await basketStore.GetAsync(userId, ct);
        if (basket is null || basket.Items.Count == 0)
            return Result<OrderResponse>.Failure(OrderErrors.EmptyBasket);

        // The basket (Redis) is only a list of "what" and "how many".
        // Name, picture and price always come from the database, so a stale
        // basket can never produce a stale price or an order for a deleted product.

        var productIds = basket.Items.Select(i => i.ProductId).Distinct().ToList();
        var products =await productRepository.ListAsync(
            new ProductForBasketSpecification(productIds), ct);
        var productsById = products.ToDictionary(p => p.Id);

        var lines = new List<(Guid ProductId, string ProductName, string PictureUrl, decimal UnitPrice, int Quantity)>();
        foreach (var item in basket.Items)
        {
            if(!productsById.TryGetValue(item.ProductId,out var product))
                return Result<OrderResponse>.Failure(OrderErrors.ProductUnavailable);

            lines.Add((product.Id, product.Name, product.PictureUrl, product.Price, item.Quantity));
        }

        var createResult = Order.Create(
            Guid.NewGuid(), userId, deliveryMethod, address, lines);
        if (createResult.IsFailure)
            return Result<OrderResponse>.Failure(createResult.Error!);

        var order = createResult.Value!;
        orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync(ct);
        // The order is already saved. A Redis failure while clearing the basket must not
        // turn a successful checkout into a 500 (the user would retry and create a duplicate order).
        try
        {
            basket.Clear(); // clear basket and save it in redis 
            await basketStore.SaveAsync(basket, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException )
        {
            logger.LogWarning(
                ex, "Order {OrderId} was created but the basket of user {UserId} could not be cleared.", order.Id, userId);
        }
        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}

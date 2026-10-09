using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler
    (ICurrentUserService currentUser,IBasketStore basketStore,
    IReadRepository<UserAddress> addressRepository,
    IRepository<DeliveryMethod> deliveryMethodRepository,IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork )
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

        var lines = basket.Items
            .Select(i => (i.ProductId, i.ProductName, i.PictureUrl, i.UnitPrice, i.Quantity)).ToList();

        var createResult = Order.Create(
            Guid.NewGuid(), userId, deliveryMethod, address, lines);
        if (createResult.IsFailure)
            return Result<OrderResponse>.Failure(createResult.Error);

        var order = createResult.Value;
        orderRepository.Add(order);
        await unitOfWork.SaveChangesAsync(ct);

        basket.Clear(); // clear basket and save it in redis 
        await basketStore.SaveAsync(basket, ct);

        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}

using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(ICurrentUserService currentUser,
    IRepository<Order> orderRepository,IUnitOfWork unitOfWork)
    : IRequestHandle<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken ct = default)
    {
        if (currentUser.UserId == null)
            return Result<OrderResponse>.Failure(OrderErrors.Unauthorized);

        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByIdForUserSpecification(request.OrderId, currentUser.UserId.Value, tracking: true), ct);
        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFound);

        var cancelOrder = order.Cancel();
        if (cancelOrder.IsFailure)
            return Result<OrderResponse>.Failure(cancelOrder.Error);

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}

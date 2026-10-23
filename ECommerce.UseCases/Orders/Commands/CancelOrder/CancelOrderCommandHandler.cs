using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;
using ECommerce.UseCases.Orders.Specifications;
using Microsoft.Extensions.Logging;

namespace ECommerce.UseCases.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    ICurrentUserService currentUser,
    IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork,
    IPaymentService paymentService,
    ILogger<CancelOrderCommandHandler> logger)
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
            return Result<OrderResponse>.Failure(cancelOrder.Error!);

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(ct);

        // The order is already cancelled in our database. Closing the Stripe PaymentIntent is best effort:
        // if it fails (or the customer pays in the meantime) the webhook refunds the payment automatically.

        if (!string.IsNullOrWhiteSpace(order.PaymentIntentId))
        {
            var cancelIntent = await paymentService.CancelPaymentIntentAsync(order.PaymentIntentId, order.Id, ct);
            if (cancelIntent.IsFailure)
            {
                logger.LogWarning(
                    "Order {OrderId} was cancelled but PaymentIntent {IntentId} could not be cancelled: {Error}",
                    order.Id, order.PaymentIntentId, cancelIntent.Error!.Code);
            }
        }

        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}

using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Specifications;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Orders.Commands.CancelOrder;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ECommerce.UseCases.Tests.Orders;

public class CancelOrderCommandHandlerTests
{
    private readonly Guid userId = Guid.NewGuid();

    private readonly ICurrentUserService currentUser = Substitute.For<ICurrentUserService>();
    private readonly IRepository<Order> orderRepository = Substitute.For<IRepository<Order>>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPaymentService paymentService = Substitute.For<IPaymentService>();

    private readonly CancelOrderCommandHandler handler;

    public CancelOrderCommandHandlerTests()
    {
        currentUser.UserId.Returns(userId);

        handler = new CancelOrderCommandHandler(
            currentUser,
            orderRepository,
            unitOfWork,
            paymentService,
            NullLogger<CancelOrderCommandHandler>.Instance);
    }

    private Order GivenOrder(string? paymentIntentId)
    {
        var order = TestOrders.Pending(userId, paymentIntentId);
        orderRepository
            .FirstOrDefaultAsync(Arg.Any<ISpecification<Order>>(), Arg.Any<CancellationToken>())
            .Returns(order);
        return order;
    }

    [Fact]
    public async Task Cancelling_an_order_with_a_payment_intent_cancels_it_in_stripe()
    {
        var order = GivenOrder("pi_123");
        paymentService
            .CancelPaymentIntentAsync("pi_123", order.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await handler.Handle(new CancelOrderCommand(order.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        await paymentService.Received(1)
            .CancelPaymentIntentAsync("pi_123", order.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancelling_an_order_without_a_payment_intent_does_not_call_stripe()
    {
        var order = GivenOrder(paymentIntentId: null);

        var result = await handler.Handle(new CancelOrderCommand(order.Id));

        Assert.True(result.IsSuccess);
        await paymentService.DidNotReceiveWithAnyArgs()
            .CancelPaymentIntentAsync(default!, default, default);
    }

    [Fact]
    public async Task Order_stays_cancelled_even_if_stripe_cancellation_fails()
    {
        var order = GivenOrder("pi_123");
        paymentService
            .CancelPaymentIntentAsync("pi_123", order.Id, Arg.Any<CancellationToken>())
            .Returns(Result.Failure(OrderErrors.PaymentCancellationFailed));

        var result = await handler.Handle(new CancelOrderCommand(order.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
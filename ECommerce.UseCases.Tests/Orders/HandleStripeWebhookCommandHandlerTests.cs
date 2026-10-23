using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Specifications;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ECommerce.UseCases.Tests.Orders;

public class HandleStripeWebhookCommandHandlerTests
{
    private const string IntentId = "pi_123";

    private readonly Guid userId = Guid.NewGuid();

    private readonly IPaymentService paymentService = Substitute.For<IPaymentService>();
    private readonly IRepository<Order> orderRepository = Substitute.For<IRepository<Order>>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly HandleStripeWebhookCommandHandler handler;

    public HandleStripeWebhookCommandHandlerTests()
    {
        handler = new HandleStripeWebhookCommandHandler(
            paymentService,
            orderRepository,
            unitOfWork,
            NullLogger<HandleStripeWebhookCommandHandler>.Instance);
    }

    private void GivenEvent(string eventType) =>
        paymentService
            .ParseWebhook(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result<StripeWebhookEvent>.Success(new StripeWebhookEvent(eventType, IntentId)));

    private void GivenOrderForIntent(Order? order) =>
        orderRepository
            .FirstOrDefaultAsync(Arg.Any<ISpecification<Order>>(), Arg.Any<CancellationToken>())
            .Returns(order);

    private Task<Result> SendAsync() =>
        handler.Handle(new HandleStripeWebhookCommand("{}", "t=1,v1=signature"));

    [Fact]
    public async Task Payment_succeeded_marks_a_pending_order_as_paid()
    {
        var order = TestOrders.Pending(userId, IntentId);
        GivenEvent("payment_intent.succeeded");
        GivenOrderForIntent(order);

        var result = await SendAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Processing, order.Status);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await paymentService.DidNotReceiveWithAnyArgs().RefundPaymentIntentAsync(default!, default);
    }

    [Fact]
    public async Task Payment_for_a_cancelled_order_is_refunded_automatically()
    {
        var order = TestOrders.Pending(userId, IntentId);
        order.Cancel();
        GivenEvent("payment_intent.succeeded");
        GivenOrderForIntent(order);
        paymentService
            .RefundPaymentIntentAsync(IntentId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await SendAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        await paymentService.Received(1)
            .RefundPaymentIntentAsync(IntentId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task A_failed_refund_fails_the_webhook_so_stripe_retries_it()
    {
        var order = TestOrders.Pending(userId, IntentId);
        order.Cancel();
        GivenEvent("payment_intent.succeeded");
        GivenOrderForIntent(order);
        paymentService
            .RefundPaymentIntentAsync(IntentId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure(OrderErrors.RefundFailed));

        var result = await SendAsync();

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.RefundFailed.Code, result.Error!.Code);
    }

    [Fact]
    public async Task A_duplicate_payment_succeeded_event_is_harmless()
    {
        var order = TestOrders.Pending(userId, IntentId);
        GivenEvent("payment_intent.succeeded");
        GivenOrderForIntent(order);

        var first = await SendAsync();
        var second = await SendAsync();

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal(OrderStatus.Processing, order.Status);
        await paymentService.DidNotReceiveWithAnyArgs().RefundPaymentIntentAsync(default!, default);
    }

    [Fact]
    public async Task An_unknown_payment_intent_is_ignored()
    {
        GivenEvent("payment_intent.succeeded");
        GivenOrderForIntent(null);

        var result = await SendAsync();

        Assert.True(result.IsSuccess);
        await paymentService.DidNotReceiveWithAnyArgs().RefundPaymentIntentAsync(default!, default);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}
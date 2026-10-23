using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Specifications;
using Microsoft.Extensions.Logging;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed class HandleStripeWebhookCommandHandler(
    IPaymentService paymentService, 
    IRepository<Order> orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<HandleStripeWebhookCommandHandler> logger)
    : IRequestHandle<HandleStripeWebhookCommand, Result>
{
    private const string PaymentIntentSucceeded= "payment_intent.succeeded";
    private const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
    public async Task<Result> Handle(HandleStripeWebhookCommand request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Signature))
            return Result.Failure(OrderErrors.InvalidWebhook);

        var parsed = paymentService.ParseWebhook(request.Payload, request.Signature);
        if (parsed.IsFailure)
            return Result.Failure(parsed.Error!);

        var stripeEvent = parsed.Value!;

        if(string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
            return Result.Success();

        switch (stripeEvent.EventType)
        {
            case PaymentIntentSucceeded:
                return await OnPaymentSucceededAsync(stripeEvent.PaymentIntentId, ct);

            case PaymentIntentPaymentFailed:
                logger.LogWarning(
                    "Payment failed for PaymentIntent {IntentId}. The order stays Pending.",
                    stripeEvent.PaymentIntentId);
                return Result.Success();
            default:
                return Result.Success();
        }
    }
    private async Task<Result> OnPaymentSucceededAsync(string paymentIntentId, CancellationToken ct)
    {
        var order=orderRepository.FirstOrDefaultAsync(new
            OrderByPaymentIntentSpecification(paymentIntentId,tracking:true), ct).Result;
        if (order is null)
        {
            logger.LogWarning("PaymentSucceeded: no order for PaymentIntent {IntentId}", paymentIntentId);
            return Result.Success();
        }
        var paid = order.MarkAsPaid(paymentIntentId);
        if (paid.IsSuccess)
        {
            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation(
                "Payment succeeded for Order {OrderId}, Intent {IntentId}", order.Id, paymentIntentId);
            return Result.Success();
        }
        if(paid.Error!.Code == OrderErrors.CannotPayCancelled.Code)
        {
            // The customer paid an order that was already cancelled: give the money back.
            // A failure here is returned so Stripe retries the webhook (the refund is idempotent).
            logger.LogWarning(
                "PaymentIntent {IntentId} succeeded for cancelled Order {OrderId}. Refunding.",
                paymentIntentId, order.Id);
            return await paymentService.RefundPaymentIntentAsync(paymentIntentId, ct);
        }
        logger.LogWarning(
            "PaymentSucceeded MarkAsPaid failed for {OrderId}: {Error}", order.Id, paid.Error.Code);
        return Result.Success();
    }
}

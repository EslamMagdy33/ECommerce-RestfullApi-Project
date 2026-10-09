using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed class HandleStripeWebhookCommandHandler(IPaymentService paymentService)
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
            return Result.Failure(parsed.Error);

        var stripeEvent = parsed.Value;

        switch (stripeEvent.EventType)
        {
            case PaymentIntentSucceeded:
                if (!string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
                    await paymentService.PaymentSucceeded(stripeEvent.PaymentIntentId, ct);
                break;

            case PaymentIntentPaymentFailed:
                if (!string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
                    await paymentService.PaymentFailed(stripeEvent.PaymentIntentId, ct);
                break;
        }

        return Result.Success();
    }
}

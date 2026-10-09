using ECommerce.Domain.Common;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit, string currency, Guid orderId, CancellationToken ct = default);


    Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,long amountInSmallestUnit,Guid orderId,CancellationToken ct = default);


    Result<StripeWebhookEvent> ParseWebhook(string jsonPayload, string stripeSignature);

    Task PaymentSucceeded(string paymentIntentId, CancellationToken ct = default);

    Task PaymentFailed(string paymentIntentId, CancellationToken ct = default);

}


public sealed record StripeWebhookEvent(
    string EventType,string PaymentIntentId);
public sealed record PaymentIntentResult(
    string PaymentIntentId,string ClientSecret,string Status,string PublishableKey);
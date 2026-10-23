using ECommerce.Domain.Common;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
        long amountInSmallestUnit, string currency, Guid orderId, CancellationToken ct = default);
    Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(
        string paymentIntentId,long amountInSmallestUnit,Guid orderId,CancellationToken ct = default);

    /// <summary>Closes the PaymentIntent so the customer can no longer pay for a cancelled order.</summary>
    Task<Result> CancelPaymentIntentAsync(string paymentIntentId, Guid orderId, CancellationToken ct = default);
    /// <summary>Gives the money back for a payment that succeeded but must not be kept.</summary>
    Task<Result> RefundPaymentIntentAsync(string paymentIntentId, CancellationToken ct = default);
    Result<StripeWebhookEvent> ParseWebhook(string jsonPayload, string stripeSignature);

}


public sealed record StripeWebhookEvent(
    string EventType,string PaymentIntentId);
public sealed record PaymentIntentResult(
    string PaymentIntentId,string ClientSecret,string Status,string PublishableKey);
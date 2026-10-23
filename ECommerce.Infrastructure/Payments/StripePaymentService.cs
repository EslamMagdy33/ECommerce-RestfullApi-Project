using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Orders.Specifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.Infrastructure.Payments;

public sealed class StripePaymentService
    (IOptions<StripeSettings> StripeOptions,ILogger<StripePaymentService> logger,
    IRepository<Order> orderRepository,IUnitOfWork unitOfWork)
    : IPaymentService
{
    private readonly StripeSettings _settings = StripeOptions.Value;
    public async Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(long amountInSmallestUnit, string currency, Guid orderId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_settings.SecretKey))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if (amountInSmallestUnit < 1)
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var service = new PaymentIntentService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment_intent_order_{orderId:D}"
        };

        try
        {
            var paymentIntent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = amountInSmallestUnit,
                Currency = currency.ToLowerInvariant(),
                PaymentMethodTypes = ["card"],
                Metadata = new Dictionary<string, string>
                {
                    ["OrderId"] = orderId.ToString("D")
                }
            },requestOptions,ct);
            logger.LogInformation("Created PaymentIntent {Id} for Order {OrderId}", paymentIntent.Id, orderId);
            return ToResult(paymentIntent);
        }
        catch (StripeException exception)
        {
            logger.LogError(exception, "Stripe create failed for Order {OrderId}", orderId);
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }

    }

    public async Task<Result<PaymentIntentResult>> UpdatePaymentIntentAsync(string paymentIntentId,
        long amountInSmallestUnit, Guid orderId, CancellationToken ct = default)
    {
        if(string.IsNullOrEmpty(_settings.SecretKey))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        if(string.IsNullOrEmpty(paymentIntentId))
            return Result<PaymentIntentResult>.Failure(OrderErrors.InvalidPaymentIntent);
        if (amountInSmallestUnit < 1)
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        StripeConfiguration.ApiKey = _settings.SecretKey;

        var service = new PaymentIntentService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = $"payment_intent_order_{orderId:D}_update"
        };
        try
        {
            var paymentIntent = await service.UpdateAsync(
                paymentIntentId,new PaymentIntentUpdateOptions { Amount = amountInSmallestUnit },
                requestOptions,ct);

            logger.LogInformation(
                "Updated PaymentIntent {Id} for Order {OrderId}",
                paymentIntent.Id, orderId);

            return ToResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe update failed for Order {OrderId}", orderId);
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);
        }
    }
    public async Task<Result> CancelPaymentIntentAsync(string paymentIntentId, Guid orderId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_settings.SecretKey) && string.IsNullOrWhiteSpace(paymentIntentId))
            return Result.Failure(OrderErrors.PaymentCancellationFailed);

        var requestOption = new RequestOptions
        {
            ApiKey = _settings.SecretKey,
            IdempotencyKey = $"payment_intent_order_{orderId:D}_cancel"
        };
        try
        {
            var service = new PaymentIntentService();
            await service.CancelAsync(
                paymentIntentId,
                new PaymentIntentCancelOptions { CancellationReason = "requested_by_customer" },
                requestOption, ct);
            logger.LogInformation("Cancelled PaymentIntent {Id} for Order {OrderId}", paymentIntentId, orderId);
            return Result.Success();
        }
        catch (StripeException ex)
        {
            // Typical reason: the customer already paid. The webhook refunds that case automatically.
            logger.LogWarning(ex, "Stripe cancel failed for PaymentIntent {Id} (Order {OrderId})", paymentIntentId, orderId);
            return Result.Failure(OrderErrors.PaymentCancellationFailed);
        }

    }

    public async Task<Result> RefundPaymentIntentAsync(string paymentIntentId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_settings.SecretKey) && string.IsNullOrWhiteSpace(paymentIntentId))
            return Result.Failure(OrderErrors.RefundFailed);
        var requestOptions = new RequestOptions
        {
            ApiKey = _settings.SecretKey,
            // Same key for the same PaymentIntent: webhook retries can never refund twice
            IdempotencyKey = $"refund_{paymentIntentId}"
        };
        try
        {
            var service = new RefundService();
            var refund = await service.CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Reason = "requested_by_customer"
            }, requestOptions, ct);
            logger.LogInformation("Refunded PaymentIntent {IntentId} (Refund {RefundId})", paymentIntentId, refund.Id);
            return Result.Success();
        }
        catch (StripeException ex) when (ex.StripeError?.Code == "charge_already_refunded")
        {
            logger.LogInformation("PaymentIntent {IntentId} was already refunded", paymentIntentId);
            return Result.Success();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe refund failed for PaymentIntent {IntentId}", paymentIntentId);
            return Result.Failure(OrderErrors.RefundFailed);
        }
    }
    public Result<StripeWebhookEvent> ParseWebhook(string jsonPayload, string stripeSignature)
    {
        if(string.IsNullOrWhiteSpace(jsonPayload)|| string.IsNullOrWhiteSpace(stripeSignature)||
            string.IsNullOrWhiteSpace(_settings.WebhookSecret))
            return Result<StripeWebhookEvent>.Failure(OrderErrors.InvalidWebhook);

        try
        {
            var stripeEvent= EventUtility.ConstructEvent(jsonPayload, stripeSignature,_settings.WebhookSecret);
            logger.LogInformation("Webhook recieved: {Type} | EventId:{Id}", stripeEvent.Type, stripeEvent.Id);

            var paymentIntentId = stripeEvent.Data.Object is PaymentIntent intent ?intent.Id: string.Empty;

            return Result<StripeWebhookEvent>.Success(new StripeWebhookEvent(stripeEvent.Type, paymentIntentId));
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Webhook signature verification failed.");
            return Result<StripeWebhookEvent>.Failure(OrderErrors.InvalidWebhook);
        }

    }
    private Result<PaymentIntentResult> ToResult(PaymentIntent paymentIntent)
    {
        if (string.IsNullOrWhiteSpace(paymentIntent.ClientSecret))
            return Result<PaymentIntentResult>.Failure(OrderErrors.PaymentFailed);

        return Result<PaymentIntentResult>.Success(new PaymentIntentResult(
            paymentIntent.Id,
            paymentIntent.ClientSecret,
            paymentIntent.Status,
            StripeOptions.Value.PublishableKey));
    }

}


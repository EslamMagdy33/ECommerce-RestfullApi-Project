namespace ECommerce.UseCases.Orders.DTOs;

public sealed record PaymentClientSecretResponse(
    Guid OrderId,string PaymentIntentId, string ClientSecret,string Status,string PublishableKey);

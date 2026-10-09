using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;

namespace ECommerce.UseCases.Orders.Commands.CreateOrderPayment;

public sealed record CreateOrderPaymentCommand(Guid OrderId)
    : ICommand<Result<PaymentClientSecretResponse>>;
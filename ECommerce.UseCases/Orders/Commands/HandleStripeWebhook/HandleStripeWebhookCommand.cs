using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed record HandleStripeWebhookCommand(string Payload, string Signature)
    : ICommand<Result>;

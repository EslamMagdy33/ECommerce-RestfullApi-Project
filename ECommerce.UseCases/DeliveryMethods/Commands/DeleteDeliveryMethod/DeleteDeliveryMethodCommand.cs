using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Commands.DeleteDeliveryMethod;

public sealed record DeleteDeliveryMethodCommand(Guid Id)
    : ICommand<Result>;

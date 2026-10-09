using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid ShippingAddressId, Guid DeliverMethodId)
    : ICommand<Result<OrderResponse>>;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;

namespace ECommerce.UseCases.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : ICommand<Result<OrderResponse>>;
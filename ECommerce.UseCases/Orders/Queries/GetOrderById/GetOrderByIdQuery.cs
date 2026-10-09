using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;

namespace ECommerce.UseCases.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<Result<OrderResponse>>;

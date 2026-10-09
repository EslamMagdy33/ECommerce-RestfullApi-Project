using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;

namespace ECommerce.UseCases.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(
    int PageNumber = 1,int PageSize = 10) : IQuery<Result<PagedResult<OrderResponse>>>;

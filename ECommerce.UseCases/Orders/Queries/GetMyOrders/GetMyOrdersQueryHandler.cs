using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler
    (ICurrentUserService currentUser,IRepository<Order> orderRepository)
    : IRequestHandle<GetMyOrdersQuery, Result<PagedResult<OrderResponse>>>
{
    public async Task<Result<PagedResult<OrderResponse>>> Handle(GetMyOrdersQuery request, CancellationToken ct = default)
    {
        if (currentUser.UserId == null)
            return Result<PagedResult<OrderResponse>>.Failure(OrderErrors.Unauthorized);

        var pageNumber= request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize is < 1 or > 50 ? 10 : request.PageSize;

        var userId = currentUser.UserId.Value;

        var totalCount = await orderRepository.CountAsync(
            new OrdersByUserIdCountSpecification(userId), ct);

        var orders = await orderRepository.ListAsync(
            new OrdersByUserIdPagedSpecification(userId, pageNumber, pageSize), ct);

        var items = orders.Select(OrderMappings.ToResponse).ToList();

        return Result<PagedResult<OrderResponse>>.Success(
            new PagedResult<OrderResponse>(items, totalCount));
    }
}

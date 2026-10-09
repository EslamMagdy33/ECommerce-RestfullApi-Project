using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.DTOs;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
    (ICurrentUserService currentUser, IRepository<Order> orderRepository)
    : IRequestHandle<GetOrderByIdQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken ct = default)
    {
        if(currentUser.UserId==null)
            return  Result<OrderResponse>.Failure(OrderErrors.Unauthorized);

        var order = await orderRepository.FirstOrDefaultAsync(
            new OrderByIdForUserSpecification(request.OrderId, currentUser.UserId.Value),ct);
        if(order == null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFound);

        return Result<OrderResponse>.Success(OrderMappings.ToResponse(order));
    }
}

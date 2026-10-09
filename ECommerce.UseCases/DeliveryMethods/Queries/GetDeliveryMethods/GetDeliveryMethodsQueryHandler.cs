using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;

public sealed class GetDeliveryMethodsQueryHandler(IReadRepository<DeliveryMethod> repository)
    : IRequestHandle<GetDeliveryMethodsQuery, Result<IReadOnlyList<DeliveryMethodResponse>>>
{
    public async Task<Result<IReadOnlyList<DeliveryMethodResponse>>> Handle(GetDeliveryMethodsQuery request, CancellationToken ct = default)
    {
        var items = await repository.ListAsync(new DeliveryMethodsListSpecification(request.AvailableOnly,request.Search), ct);
       
        return Result<IReadOnlyList<DeliveryMethodResponse>>.Success(items);

    }
}

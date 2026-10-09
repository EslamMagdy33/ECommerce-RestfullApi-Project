using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.DeliveryMethods.DTOs;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethodById;

public sealed class GetDeliveryMethodByIdQueryHandler(IReadRepository<DeliveryMethod> repository)
    : IRequestHandle<GetDeliveryMethodByIdQuery, Result<DeliveryMethodResponse>>
{
    public async Task<Result<DeliveryMethodResponse>> Handle(GetDeliveryMethodByIdQuery request,CancellationToken ct)
    {
        var item = await repository.FirstOrDefaultAsync(
            new DeliveryMethodByIdSpecification(request.Id),ct);

        if (item is null)
            return Result<DeliveryMethodResponse>.Failure(DeliveryMethodErrors.NotFound);

        return Result<DeliveryMethodResponse>.Success(item);
    }
}

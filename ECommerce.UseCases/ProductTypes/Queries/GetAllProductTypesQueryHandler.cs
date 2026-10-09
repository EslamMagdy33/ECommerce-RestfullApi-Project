using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.ProductTypes.DTOs;
using ECommerce.UseCases.ProductTypes.Specifications;

namespace ECommerce.UseCases.ProductTypes.Queries;

public sealed class GetAllProductTypesQueryHandler(IReadRepository<ProductType> repository)
    : IRequestHandle<GetAllProductTypesQuery, Result<IReadOnlyList<GetAllProductTypesResponse>>>
{
    public async Task<Result<IReadOnlyList<GetAllProductTypesResponse>>> Handle(GetAllProductTypesQuery request, CancellationToken ct = default)
    {
        var types = await repository.ListAsync(new TypesListSpecification(), ct);
        return Result<IReadOnlyList<GetAllProductTypesResponse>>.Success(types);
    }
}

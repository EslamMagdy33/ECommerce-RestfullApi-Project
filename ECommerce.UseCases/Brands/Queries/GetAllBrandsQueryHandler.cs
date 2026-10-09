using ECommerce.UseCases.Brands.DTOs;
using ECommerce.UseCases.Brands;
using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.UseCases.Brands.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Brands.Queries;

public sealed class GetAllBrandsQueryHandler(IReadRepository<ProductBrand> repository):
    IRequestHandle<GetAllBrandsQuery, Result<IReadOnlyList<GetAllBrandsResponse>>>
{
    public async Task<Result<IReadOnlyList<GetAllBrandsResponse>>> Handle(GetAllBrandsQuery request,CancellationToken ct=default)
    {
        var Brands=await repository.ListAsync(new BrandsListSpecification(), ct);
        return Result<IReadOnlyList<GetAllBrandsResponse>>.Success(Brands);
    }
}

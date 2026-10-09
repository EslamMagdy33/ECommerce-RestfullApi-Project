using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries.GetAllProducts;

public sealed class GetAllProductsQueryHandler(IReadRepository<Product> repository)
    : IRequestHandle<GetAllBrandsQuery,Result<IReadOnlyList<GetAllProductsResponse>>>
{
    public async Task<Result<IReadOnlyList<GetAllProductsResponse>>> Handle(GetAllBrandsQuery request, CancellationToken ct = default)
    {
        var products=await repository.ListAsync(new ProductsListSpecification(), ct);
        return Result<IReadOnlyList<GetAllProductsResponse>>.Success(products);
    }
}

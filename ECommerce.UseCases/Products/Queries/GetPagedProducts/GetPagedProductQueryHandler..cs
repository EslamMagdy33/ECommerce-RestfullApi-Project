using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries.GetPagedProducts;

public sealed class GetPagedProductQueryHandler(IReadRepository<Product> repository) 
    : IRequestHandle<GetPagedProductQuery, Result<PagedResult<GetAllProductsResponse>>>
{
    public async Task<Result<PagedResult<GetAllProductsResponse>>> Handle(GetPagedProductQuery request, CancellationToken ct = default)
    {
        var countSpec = new ProductsPagedSpecification(
            request.Search,
            request.BrandId,
            request.TypeId
            );
        var listSpec=new ProductsPagedSpecification(
            request.Search,
            request.BrandId,
            request.TypeId,
            request.SortBy,
            request.SortDescending,
            request.PageNumber,
            request.PageSize
            );
        var totalCount=await repository.CountAsync(countSpec, ct);
        var items = await repository.ListAsync(listSpec, ct);

        return Result<PagedResult<GetAllProductsResponse>>.Success(
            new PagedResult<GetAllProductsResponse>(items, totalCount));
    }
}

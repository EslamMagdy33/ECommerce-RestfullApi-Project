using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries.GetByIdProduct;

public sealed class GetByIdProductQueryHandler(IReadRepository<Product> repository)
    :IRequestHandle<GetByIdProductQuery,Result<GetByIdProductResponse>>
{
    public async Task<Result<GetByIdProductResponse>> Handle(GetByIdProductQuery request, CancellationToken ct = default)
    {
        var product=await repository.FirstOrDefaultAsync(new ProductByIdSpecification(request.Id),ct);
        if (product == null)
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);
        return product;
    }
}

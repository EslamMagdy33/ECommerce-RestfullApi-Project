using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.DTOs;
using ECommerce.UseCases.Products.Enums;

namespace ECommerce.UseCases.Products.Queries.GetPagedProducts;

public sealed record GetPagedProductQuery(
    int PageNumber = 1,
    int PageSize = 5,
    string? Search = null,
    Guid? BrandId = null,
    Guid? TypeId = null,
    ProductSortField? SortBy = ProductSortField.Name,
    bool SortDescending = false
    ) : IQuery<Result<PagedResult<GetAllProductsResponse>>>;

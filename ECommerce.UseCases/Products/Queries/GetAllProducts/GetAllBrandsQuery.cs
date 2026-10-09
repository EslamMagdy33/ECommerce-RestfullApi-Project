using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.DTOs;

namespace ECommerce.UseCases.Products.Queries.GetAllProducts;

public sealed record GetAllBrandsQuery : IQuery<Result<IReadOnlyList<GetAllProductsResponse>>>;


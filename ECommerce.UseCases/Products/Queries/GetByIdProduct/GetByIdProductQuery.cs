using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.DTOs;

namespace ECommerce.UseCases.Products.Queries.GetByIdProduct;

public sealed record GetByIdProductQuery(Guid Id) : IQuery<Result<GetByIdProductResponse>>;
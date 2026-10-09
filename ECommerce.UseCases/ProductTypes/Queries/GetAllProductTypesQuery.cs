using ECommerce.Domain.Common;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.ProductTypes.DTOs;

namespace ECommerce.UseCases.ProductTypes.Queries;

public sealed record GetAllProductTypesQuery : IQuery<Result<IReadOnlyList<GetAllProductTypesResponse>>>;
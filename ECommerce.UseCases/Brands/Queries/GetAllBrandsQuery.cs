using ECommerce.Domain.Common;
using ECommerce.UseCases.Brands.DTOs;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Brands.Queries;

public sealed record GetAllBrandsQuery : IQuery<Result<IReadOnlyList<GetAllBrandsResponse>>>;

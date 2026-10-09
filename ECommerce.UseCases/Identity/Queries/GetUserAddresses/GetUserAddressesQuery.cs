using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Queries.GetUserAddresses;

public sealed record GetUserAddressesQuery: IQuery<Result<IReadOnlyList<UserAddressResponse>>>;   

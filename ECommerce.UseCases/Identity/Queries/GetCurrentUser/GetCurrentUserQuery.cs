using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<Result<UserProfileResponse>>;

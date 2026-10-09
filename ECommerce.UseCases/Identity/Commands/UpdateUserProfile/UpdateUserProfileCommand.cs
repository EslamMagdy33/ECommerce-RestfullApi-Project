using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(string? DisplayName)
    : ICommand<Result<UserProfileResponse>>;

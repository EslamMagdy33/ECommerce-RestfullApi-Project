using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler(
    ICurrentUserService currentUserService,
    IIdentityService identityService)
    : IRequestHandle<UpdateUserProfileCommand, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(UpdateUserProfileCommand request, CancellationToken ct = default)
    {
        if(currentUserService.UserId == null)
            return Result<UserProfileResponse>.Failure(IdentityErrors.InvalidCredentials);

        var updateResult = await identityService.UpdateProfileAsync(
            currentUserService.UserId.Value,
            request.DisplayName,
            ct);

        if(updateResult.IsFailure)
            return Result<UserProfileResponse>.Failure(updateResult.Error);

        var user= updateResult.Value;

        return Result<UserProfileResponse>.Success(
            new UserProfileResponse(user.UserId, user.Email, user.DisplayName));
    }
}

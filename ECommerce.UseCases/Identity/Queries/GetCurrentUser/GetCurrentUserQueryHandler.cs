using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUserService userService,
    IIdentityService identityService)
    : IRequestHandle<GetCurrentUserQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(GetCurrentUserQuery request, CancellationToken ct = default)
    {
        if(userService.UserId is null)
            return Result<UserProfileResponse>.Failure(IdentityErrors.InvalidCredentials);

        var userResult = await identityService.GetUserByIdAsync(userService.UserId.Value, ct);
        if(userResult.IsFailure)
            return Result<UserProfileResponse>.Failure(userResult.Error);

        var user = userResult.Value;

        return Result<UserProfileResponse>.Success(new UserProfileResponse(user.UserId, user.Email, user.DisplayName));

    }
}

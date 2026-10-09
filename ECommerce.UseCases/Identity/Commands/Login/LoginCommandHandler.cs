using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IJwtGenerator jwtTokenGenerator,
    IRefreshTokenService refreshTokenService)
    : IRequestHandle<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct = default)
    {
        var validateResult=await identityService.ValidateCredentialsAsync(
            request.Email, request.Password, ct);

        if(validateResult.IsFailure)
        {
            var code = validateResult.Error.Code;

            var error =
                code == IdentityErrors.EmailNotConfirmed.Code ? IdentityErrors.EmailNotConfirmed :
                code == IdentityErrors.AccountLockedOut.Code ? IdentityErrors.AccountLockedOut :
                IdentityErrors.InvalidCredentials;

            return Result<AuthResponse>.Failure(error);
        }
        var user = validateResult.Value;

        var roles = await identityService.GetRolesAsync(user.UserId, ct);

        var accessToken = jwtTokenGenerator.GenerateToken(user.UserId, user.Email,user.DisplayName, roles);

        var refreshToken = await refreshTokenService.IssueAsync(user.UserId, ct);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.UserId,
            user.Email,
            user.DisplayName,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken.refreshToken,
            refreshToken.expiresAtUtc));
    }
}

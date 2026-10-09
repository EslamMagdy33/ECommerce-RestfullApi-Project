using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

internal class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokenService,
    IIdentityService identityService,
    IJwtGenerator jwtTokenGenerator) : IRequestHandle<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidRefreshToken);

        var rotateResult = await refreshTokenService.RotateAsync(
            request.RefreshToken.Trim(),ct);
        if (rotateResult.IsFailure)
            return Result<AuthResponse>.Failure(rotateResult.Error);

        var refresh = rotateResult.Value;

        var userResult = await identityService.GetUserByIdAsync(refresh.userId,ct);
        if (userResult.IsFailure)
            return Result<AuthResponse>.Failure(IdentityErrors.InvalidRefreshToken);

        var user = userResult.Value;

        var roles = await identityService.GetRolesAsync(user.UserId, ct);
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.UserId, user.Email, user.DisplayName, roles);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.UserId,
            user.Email,
            user.DisplayName,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refresh.refreshToken,
            refresh.expiresAtUtc));

    }
}

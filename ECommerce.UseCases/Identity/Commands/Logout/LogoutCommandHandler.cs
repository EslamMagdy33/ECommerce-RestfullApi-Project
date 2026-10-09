using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.Logout;

public sealed class LogoutCommandHandler(IRefreshTokenService refreshTokenService)
    : IRequestHandle<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result.Failure(IdentityErrors.InvalidRefreshToken);

        return await refreshTokenService.RevokeAsync(
            request.RefreshToken.Trim(), ct);
    }
}

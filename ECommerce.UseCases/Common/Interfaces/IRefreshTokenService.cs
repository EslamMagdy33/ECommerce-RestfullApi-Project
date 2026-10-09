using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Models;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssueResult> IssueAsync(Guid userId,CancellationToken ct=default);
    Task<Result<RefreshTokenIssueResult>> RotateAsync(string refreshToken, CancellationToken ct = default);
    Task<Result> RevokeAsync(string refreshToken, CancellationToken ct = default);
}

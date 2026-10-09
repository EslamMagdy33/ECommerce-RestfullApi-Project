using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Models;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<AuthUserSnapshot>> CreateUserAsync(string email, string password, string? displayName, CancellationToken ct = default);
    Task<Result<AuthUserSnapshot>> GetUserByEmailAsync(string email,CancellationToken ct = default);
    Task<Result<AuthUserSnapshot>> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);

    Task<Result<AuthUserSnapshot>> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Task<Result<AuthUserSnapshot>> UpdateProfileAsync(Guid userId, string? displayName, CancellationToken ct = default);
    Task<Result> ConfirmEmailAsync(string email,CancellationToken ct = default);
    Task<bool> IsEmailConfirmedAsync(string email,CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId,CancellationToken ct = default);

}

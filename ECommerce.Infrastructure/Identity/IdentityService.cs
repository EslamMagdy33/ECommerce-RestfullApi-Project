using ECommerce.Domain.Common;
using ECommerce.Domain.Constants;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager)
    : IIdentityService
{
    public async Task<Result<AuthUserSnapshot>> CreateUserAsync(string email, string password, string? displayName, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            DisplayName = displayName,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            if(result.Errors.Any(e=>e.Code is "DuplicateUserName" or "DuplicateEmail"))
            {
                return Result<AuthUserSnapshot>.Failure(IdentityErrors.EmailAlreadyExists);
            }
            var msg = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.CreateFailed(msg));
        }
        var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
        if (!roleResult.Succeeded)
        {
            var msg = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.CreateFailed(msg));
        }

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }
    public async Task<Result<AuthUserSnapshot>> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.InvalidCredentials);

        if(await userManager.IsLockedOutAsync(user))
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.AccountLockedOut);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
        {
            await userManager.AccessFailedAsync(user);
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.InvalidCredentials);
        }

        if (await userManager.GetAccessFailedCountAsync(user) > 0)
            await userManager.ResetAccessFailedCountAsync(user);

        if (!user.EmailConfirmed)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.EmailNotConfirmed);

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));

    }
    public async Task<Result<AuthUserSnapshot>> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        var user=await userManager.FindByEmailAsync(email);
        if(user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);
        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }
    public async Task<Result<AuthUserSnapshot>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user=await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);
        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }
    public async Task<Result<AuthUserSnapshot>> UpdateProfileAsync(Guid userId, string? displayName, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.UserNotFound);

        user.DisplayName = string.IsNullOrEmpty(displayName) ? null : displayName.Trim();

        var result=await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var msg = string.Join("", result.Errors.Select(e => e.Description));
            return Result<AuthUserSnapshot>.Failure(IdentityErrors.CreateFailed(msg));
        }

        return Result<AuthUserSnapshot>.Success(
            new AuthUserSnapshot(user.Id, user.Email!, user.DisplayName));
    }
    public async Task<Result> ConfirmEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(IdentityErrors.UserNotFound);

        if(user.EmailConfirmed) //EmailAlreadyConfirmed لو رجعت ترو يبقي هي 
            return Result.Failure(IdentityErrors.EmailAlreadyConfirmed);
        // لو رجعت false يبقي محتاج EmailConfirmed 
        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(IdentityErrors.CreateFailed(
                string.Join("; ", result.Errors.Select(e => e.Description))));

    }
    public async Task<bool> IsEmailConfirmedAsync(string email, CancellationToken ct = default)
    {
       var user=await userManager.FindByEmailAsync(email);
        return user?.EmailConfirmed ?? false;
    }
    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken ct = default)
    {
       var user= await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return [];

        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }
    
}

using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler (
    IIdentityService identityService,
    IEmailVerificationCodeStore verificationCodeStore)
    : IRequestHandle<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();
        var code = request.Code.Trim();

        var userResult = await identityService.GetUserByEmailAsync(email, ct);
        if (userResult.IsFailure)
            return Result.Failure(IdentityErrors.InvalidVerificationCode);

        if(await identityService.IsEmailConfirmedAsync(email,ct))
            return Result.Failure(IdentityErrors.EmailAlreadyConfirmed);

        var isValid = await verificationCodeStore.ValidateAndConsumeAsync(email, code, ct);
        if (!isValid)
            return Result.Failure(IdentityErrors.InvalidVerificationCode);

        return await identityService.ConfirmEmailAsync(email, ct);

    }
}

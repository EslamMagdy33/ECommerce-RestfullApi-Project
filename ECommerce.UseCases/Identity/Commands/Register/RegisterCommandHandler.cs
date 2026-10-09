using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IEmailVerificationCodeStore verificationCodeStore,
    IEmailSender emailSender,
    IOptions<EmailVerificationSettings> settings)
    : IRequestHandle<RegisterCommand, Result<EmailSentResponse>>
{
    public const string RegisteredMessage =
        "Registration successful. A verification code was sent to your email. Confirm your email before logging in.";

    public const string UnconfirmedResendMessage =
        "This email is registered but not confirmed. A new verification code was sent to your email.";
    public async Task<Result<EmailSentResponse>> Handle(RegisterCommand request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        var existing=await identityService.GetUserByEmailAsync(email, ct);
        if (existing.IsSuccess)
        {
            if(await identityService.IsEmailConfirmedAsync(email,ct))
                return Result<EmailSentResponse>.Failure(IdentityErrors.EmailAlreadyExists);
            
            return await SendVerificationAsync(
                email,
                verificationCodeResent: true,
                UnconfirmedResendMessage,ct );
        }
        var createResult = await identityService.CreateUserAsync(
            email, request.Password, request.DisplayName, ct);

        if (createResult.IsFailure)
            return Result<EmailSentResponse>.Failure(createResult.Error);

        return await SendVerificationAsync(
            email, verificationCodeResent: false, RegisteredMessage, ct);
    }
    private async Task<Result<EmailSentResponse>> SendVerificationAsync(
        string email, bool verificationCodeResent,string message,CancellationToken ct)
    {
        var code = GenerateCode(settings.Value.CodeLength);
        await verificationCodeStore.SaveAsync(email, code, ct);

        var sendResul = await emailSender.SendAsync(email, "Confirm your ECommerce account",
            $"Your verification code is: {code}\n\nThis code expires in {settings.Value.ExpirationMinutes} minutes.\n\nYou cannot sign in until you confirm this email.",
            ct);
        if (sendResul.IsFailure)
            return Result<EmailSentResponse>.Failure(sendResul.Error);

        return Result<EmailSentResponse>.Success(
            new EmailSentResponse(email, verificationCodeResent, message));
    }
    private static string GenerateCode(int length)
    {
        var max = (int)Math.Pow(10, length);
        return RandomNumberGenerator.GetInt32(0, max).ToString($"D{length}");
    }
}

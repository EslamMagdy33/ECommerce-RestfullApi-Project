using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Identity;
public sealed class NoOpEmailSender(ILogger<NoOpEmailSender> logger) : IEmailSender
{
    public Task<Result> SendAsync(string toEmail,string subject,string body,CancellationToken ct = default)
    {
        logger.LogWarning(
            "\n===== DEV: EMAIL NOT SENT (NoOpEmailSender) =====\n" +
            "To: {ToEmail}\n" +
            "Subject: {Subject}\n" +
            "{Body}\n" +
            "===============================================\n" +
            "Copy the verification code from the body above into POST /auth/confirm-email.",
            toEmail,
            subject,
            body);

        return Task.FromResult(Result.Success());
    }
}
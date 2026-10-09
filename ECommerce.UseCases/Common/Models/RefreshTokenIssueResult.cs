namespace ECommerce.UseCases.Common.Models;

public sealed record RefreshTokenIssueResult(Guid userId,string refreshToken, DateTimeOffset expiresAtUtc);

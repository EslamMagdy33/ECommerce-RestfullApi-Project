namespace ECommerce.UseCases.Common.Interfaces;

public interface IEmailVerificationCodeStore
{
    Task SaveAsync(string email,string code,CancellationToken ct = default);

    Task<bool> ValidateAndConsumeAsync(
        string email, string code, CancellationToken ct = default);
}

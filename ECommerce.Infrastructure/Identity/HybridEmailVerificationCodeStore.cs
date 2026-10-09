using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Infrastructure.Identity;

public sealed class HybridEmailVerificationCodeStore(
    HybridCache cache ,IOptions<EmailVerificationSettings> settings)
    : IEmailVerificationCodeStore
{
    private const string KeyPrefix = "email-verification:";
    private readonly EmailVerificationSettings _setting = settings.Value; 
    public async Task SaveAsync(string email, string code, CancellationToken ct = default)
    {
        var key = BuildKey(email);
        var entry = new VerificationEntry(Code: code.Trim(),
            FailedAttempts: 0);
        await cache.SetAsync(key, entry, new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(_setting.ExpirationMinutes),
            LocalCacheExpiration = TimeSpan.FromMinutes(Math.Min(5, _setting.ExpirationMinutes))
        },cancellationToken:ct);
    }

    public async Task<bool> ValidateAndConsumeAsync(string email, string code, CancellationToken ct = default)
    {
        var key= BuildKey(email);
        var entry=await TryGetEntryAsync(key, ct);

        if (entry is null)
            return false;
        if(entry.FailedAttempts >= _setting.MaxFailedAttempts)
        {
            await cache.RemoveAsync(key, ct);
            return false;
        }

        var expected = Encoding.UTF8.GetBytes(entry.Code);
        var actual= Encoding.UTF8.GetBytes(code.Trim());
        if(expected.Length != actual.Length 
            || !CryptographicOperations.FixedTimeEquals(expected,actual) )
        {
            entry = entry with { FailedAttempts = entry.FailedAttempts + 1 };
            if(entry.FailedAttempts >= _setting.MaxFailedAttempts)
            {
                await cache.RemoveAsync(key, ct);
                return false;
            }
            await cache.SetAsync(key, entry, new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(_setting.ExpirationMinutes),
                LocalCacheExpiration = TimeSpan.FromMinutes(Math.Min(5, _setting.ExpirationMinutes))
            },cancellationToken:ct);

            return false;
        }
        await cache.RemoveAsync(key, ct);
        return true;

    }

    private async Task<VerificationEntry?> TryGetEntryAsync(string key,CancellationToken ct)
    {
        var found = true;

        var value = await cache.GetOrCreateAsync(
            key, _ =>
            {
                found = false;
                return ValueTask.FromResult<VerificationEntry?>(null);
            },
            new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCacheWrite
                    | HybridCacheEntryFlags.DisableDistributedCacheWrite
            },
            cancellationToken: ct);

        return found ? value : null;
    }
    private static string BuildKey(string email) => 
        KeyPrefix +email.Trim().ToLowerInvariant();
    private sealed record VerificationEntry(string Code, int FailedAttempts);
}

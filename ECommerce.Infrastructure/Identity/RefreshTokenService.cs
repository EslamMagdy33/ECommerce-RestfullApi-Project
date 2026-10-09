using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using ECommerce.UseCases.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Infrastructure.Identity;

public sealed class RefreshTokenService(AppIdentityDbContext identityDbContext,IOptions<JwtSettings> jwtSettings)
    : IRefreshTokenService
{
    public async Task<RefreshTokenIssueResult> IssueAsync(Guid userId, CancellationToken ct = default) // اصدار توكن جديد
    {
        /// 1- تولد نص التوكن العشوائي (Random Token String).
        /// 2- تنشئ Entity جديد من RefreshToken مرتبط بـ UserId وتاريخ انتهاء (مثلاً بعد 7 أيام).
        /// 3-تحفظ التوكن الجديد في قاعدة البيانات وتُرجعه.
        var (plainToken, entity) = CreateTokenEntity(userId); 
        identityDbContext.RefreshTokens.Add(entity);
        await identityDbContext.SaveChangesAsync(ct);

        return new RefreshTokenIssueResult(userId,plainToken, entity.ExpiresAtUtc);
    }

    public async Task<Result> RevokeAsync(string refreshToken, CancellationToken ct = default)
    {
        /// 1- ( معين UserId  تبحث عن التوكن (أو كل التوكنات النشطة الخاصة بـ
        /// 2- RevokedAt = DateTimeOffset.UtcNow تضع قيمة لتاريخ الإلغاء 
        /// 3-تحفظ التغيرات في قاعدة البيانات
        
        var tokenHash = HashToken(refreshToken);

        var existing = await identityDbContext.RefreshTokens
            .FirstOrDefaultAsync(x=>x.TokenHash== tokenHash,ct);
        if (existing == null)
            return Result.Failure(IdentityErrors.InvalidRefreshToken);

        if (!existing.IsRevoked)
        {
            existing.Revoke();
            await identityDbContext.SaveChangesAsync(ct);
        }

        return Result.Success();
    }

    public async Task<Result<RefreshTokenIssueResult>> RotateAsync(string refreshToken, CancellationToken ct = default)
    {
        /// 1- تبحث عن التوكن القديم المبعوث من العميل وتتأكد من أنه صالح وغير منتهي ولم يتم إبطاله (IsActive).
        /// 2-( Revokeتقوم بإبطال التوكن القديم فوراً (تعمل له 
        /// 3- .(IssueAsync تنشئ توكن جديد كلياً وتخزنه في قاعدة البيانات (تنادي 
        /// 4- تُرجع التوكنات الجديدة (Access Token جديد + Refresh Token جديد)
        var tokenHash = HashToken(refreshToken);

        var existing = await identityDbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        if (existing == null)
            return Result<RefreshTokenIssueResult>.Failure(IdentityErrors.InvalidRefreshToken);

        if (existing.IsRevoked)
        {
            await RevokeAllActiveForUserAsync(existing.UserId, ct);
            return Result<RefreshTokenIssueResult>.Failure(IdentityErrors.InvalidRefreshToken);
        }

        if (existing.IsExpired)
            return Result<RefreshTokenIssueResult>.Failure(IdentityErrors.RefreshTokenExpired);

        var (plainToken, replacement) = CreateTokenEntity(existing.UserId);
        existing.Revoke(replacement.TokenHash);
        identityDbContext.RefreshTokens.Add(replacement);
        await identityDbContext.SaveChangesAsync(ct);

        return Result<RefreshTokenIssueResult>.Success(
            new RefreshTokenIssueResult(existing.UserId, plainToken, replacement.ExpiresAtUtc));

    }

    private (string PlainToken, RefreshToken Entity) CreateTokenEntity(Guid userId)
    {
        var plainToken=GenerateSecureToken();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(jwtSettings.Value.RefreshTokenExpirationDays);
        var entity = RefreshToken.Create(Guid.NewGuid(), userId, HashToken(plainToken), expiresAt);

        return (plainToken, entity);
    }
    private static string GenerateSecureToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
    private async Task RevokeAllActiveForUserAsync(Guid userId,CancellationToken ct)
    {
        var activeTokens=await identityDbContext.RefreshTokens
            .Where(x=>x.UserId== userId && x.RevokedAtUtc==null)
            .ToListAsync();

        foreach (var activeToken in activeTokens)
            activeToken.Revoke();

        if(activeTokens.Count > 0)
            await identityDbContext.SaveChangesAsync(ct);

    }
    private static string HashToken(string Token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(Token));
        return Convert.ToHexString(hash);
    }


}

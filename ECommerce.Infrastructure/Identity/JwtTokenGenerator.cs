using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Models;
using ECommerce.UseCases.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Infrastructure.Identity;

public class JwtTokenGenerator(IOptions<JwtSettings> settings) : IJwtGenerator
{
    private readonly JwtSettings _settings = settings.Value;
    public AccessTokenResult GenerateToken(Guid userId, string email, string? displayName, IEnumerable<string> roles)
    {
        var expiresAt= DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,userId.ToString()),
            new(ClaimTypes.NameIdentifier,userId.ToString()),
            new (JwtRegisteredClaimNames.Email,email),
            new(ClaimTypes.Email,email),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };

        if (!string.IsNullOrEmpty(displayName))
            claims.Add(new Claim("display_name", displayName));

        claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));

        // Starting Building the Token ( Header - Payload - Signature )
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken
            (
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials
            );

        var written = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResult(written, expiresAt);
    }
}

using IdentityService.Config;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Infrastructure.JWT;

public class JWTProvider : IJWTProvider
{
    public string Create(User user)
    {
        string secretKey = CONFIG.JWTSecretKey;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        claims.AddRange(
            user.Roles.Select(r =>
                new Claim(ClaimTypes.Role, r.Name)
            )
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Issuer = CONFIG.JWTIssuer,
            Audience = CONFIG.JWTAudience,
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}

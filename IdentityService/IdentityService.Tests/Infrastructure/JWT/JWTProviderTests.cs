using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.JWT;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityService.Tests.Infrastructure.JWT;

public class JWTProviderTests
{
    private readonly JWTProvider _jwtProvider;
    private readonly string _testSecretKey = "thisistestsecretkeyforjwtproviderinthetestenvironmentokthankyou";
    private readonly string _testIssuer = "TestIssuer";
    private readonly string _testAudience = "TestAudience";
    private readonly int _testExpirationHours = 1;

    private readonly JwtSecurityTokenHandler _tokenHandler; 
    private readonly TokenValidationParameters _validationParameters;
    public JWTProviderTests()
    {
        // Set up environment variables for SHARED_CONFIG
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", _testSecretKey);
        Environment.SetEnvironmentVariable("JWT_ISSUER", _testIssuer);
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", _testAudience);
        Environment.SetEnvironmentVariable("JWT_EXPIRATION_HOURS", _testExpirationHours.ToString());
        _tokenHandler = new()
        {
            MapInboundClaims = false
        };
        _validationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_testSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _testIssuer,
            ValidateAudience = true,
            ValidAudience = _testAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // No leeway in token expiration
        };

        _jwtProvider = new JWTProvider();
    }

    [Fact]
    public void Create_ValidUser_GeneratesValidToken()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", FirstName = "Almouhannad", LastName = "Hafez", HashedPassword= "hashedpassword", };
        user.AddRole(Roles.USER.Name);

        // Act
        var token = _jwtProvider.Create(user);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));

        try
        {
            var principal = _tokenHandler.ValidateToken(token, _validationParameters, out SecurityToken validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            Assert.NotNull(jwtSecurityToken);
            Assert.Equal(_testIssuer, jwtSecurityToken.Issuer);
            Assert.Equal(_testAudience, jwtSecurityToken.Audiences.First());
            Assert.Equal(user.Id.ToString(), principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);
            Assert.Equal(user.Email, principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value);
            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == Roles.USER.Name);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Token validation failed: {ex.Message}");
        }
    }

    [Fact]
    public void Create_UserWithoutRoles_GeneratesValidToken()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", FirstName = "Almouhannad", LastName = "Hafez", HashedPassword = "hashedpassword", };

        // Act
        var token = _jwtProvider.Create(user);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));

        try
        {
            var principal = _tokenHandler.ValidateToken(token, _validationParameters, out SecurityToken validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            Assert.NotNull(jwtSecurityToken);
            Assert.Equal(user.Id.ToString(), principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);
            Assert.Equal(user.Email, principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value);
            Assert.DoesNotContain(principal.Claims, c => c.Type == ClaimTypes.Role);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Token validation failed: {ex.Message}");
        }
    }
}
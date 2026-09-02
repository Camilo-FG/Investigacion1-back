using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.Extensions.Options;

namespace Investigacion1_back.Tests.Auth;

public class JwtTokenServiceTests
{
    private readonly JwtTokenService _sut = new(Options.Create(new JwtOptions
    {
        Issuer = "Investigacion1",
        Audience = "Investigacion1",
        Secret = "test-jwt-secret-at-least-32-chars-long!!",
        AccessTokenMinutes = 60,
        RefreshTokenDays = 14
    }));

    [Fact]
    public void CreateAccessToken_expires_in_about_one_hour()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Role = Roles.SubscriptionL1
        };

        var before = DateTime.UtcNow;
        var jwt = _sut.CreateAccessToken(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        var lifetime = token.ValidTo - before;
        Assert.InRange(lifetime.TotalMinutes, 59, 61);

        var subject = token.Claims.First(c =>
            c.Type is ClaimTypes.NameIdentifier or JwtRegisteredClaimNames.Sub or "nameid").Value;
        var role = token.Claims.First(c =>
            c.Type is ClaimTypes.Role or "role").Value;

        Assert.Equal(user.Id.ToString(), subject);
        Assert.Equal(Roles.SubscriptionL1, role);
    }

    [Fact]
    public void CreateRefreshToken_expires_in_about_fourteen_days()
    {
        var before = DateTime.UtcNow;
        var (plain, hash, expiresAt) = _sut.CreateRefreshToken();

        Assert.False(string.IsNullOrWhiteSpace(plain));
        Assert.Equal(_sut.HashRefreshToken(plain), hash);
        Assert.InRange((expiresAt - before).TotalDays, 13.9, 14.1);
    }
}

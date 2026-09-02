using Investigacion1_back.Features.Auth.Login;
using Investigacion1_back.Shared.Contracts;

namespace Investigacion1_back.Tests.Auth;

public class LoginHandlerTests
{
    [Fact]
    public async Task Login_with_valid_credentials_returns_tokens()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("user@example.com", "Secret1");

        var result = await new LoginHandler(fx.Db, fx.Passwords, fx.Tokens)
            .Handle(new LoginRequest("user@example.com", "Secret1"), CancellationToken.None);

        var (status, body) = ResultAssert.Read<TokenResponse>(result);
        Assert.Equal(StatusCodes.Status200OK, status);
        Assert.False(string.IsNullOrWhiteSpace(body!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body.RefreshToken));
        Assert.Equal(1, fx.Db.RefreshSessions.Count());
    }

    [Fact]
    public async Task Login_with_invalid_password_returns_401()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("user@example.com", "Secret1");

        var result = await new LoginHandler(fx.Db, fx.Passwords, fx.Tokens)
            .Handle(new LoginRequest("user@example.com", "Wrong1"), CancellationToken.None);

        var (status, body) = ResultAssert.Read<ErrorResponse>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, status);
        Assert.Equal("Invalid credentials.", body!.Error);
    }

    [Fact]
    public async Task Login_with_inactive_user_returns_401()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("user@example.com", "Secret1", isActive: false);

        var result = await new LoginHandler(fx.Db, fx.Passwords, fx.Tokens)
            .Handle(new LoginRequest("user@example.com", "Secret1"), CancellationToken.None);

        var (status, _) = ResultAssert.Read<ErrorResponse>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, status);
    }
}

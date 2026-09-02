using System.Security.Claims;
using Investigacion1_back.Features.Auth.Login;
using Investigacion1_back.Features.Auth.Logout;
using Investigacion1_back.Features.Auth.Refresh;
using Investigacion1_back.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Tests.Auth;

public class RefreshAndLogoutHandlerTests
{
    [Fact]
    public async Task Refresh_rotates_tokens_and_invalidates_old_refresh()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("user@example.com", "Secret1");

        var login = await new LoginHandler(fx.Db, fx.Passwords, fx.Tokens)
            .Handle(new LoginRequest("user@example.com", "Secret1"), CancellationToken.None);
        var (_, loginBody) = ResultAssert.Read<TokenResponse>(login);

        var refresh = await new RefreshHandler(fx.Db, fx.Tokens)
            .Handle(new RefreshRequest(loginBody!.RefreshToken), CancellationToken.None);
        var (status, refreshBody) = ResultAssert.Read<TokenResponse>(refresh);

        Assert.Equal(StatusCodes.Status200OK, status);
        Assert.NotEqual(loginBody.RefreshToken, refreshBody!.RefreshToken);

        var reuse = await new RefreshHandler(fx.Db, fx.Tokens)
            .Handle(new RefreshRequest(loginBody.RefreshToken), CancellationToken.None);
        var (reuseStatus, _) = ResultAssert.Read<ErrorResponse>(reuse);
        Assert.Equal(StatusCodes.Status401Unauthorized, reuseStatus);
    }

    [Fact]
    public async Task Logout_revokes_sessions_so_old_refresh_stops_working()
    {
        await using var fx = new AuthTestFixture();
        var user = await fx.SeedUserAsync("user@example.com", "Secret1");

        var login = await new LoginHandler(fx.Db, fx.Passwords, fx.Tokens)
            .Handle(new LoginRequest("user@example.com", "Secret1"), CancellationToken.None);
        var (_, loginBody) = ResultAssert.Read<TokenResponse>(login);

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        ], authenticationType: "Bearer"));

        var logout = await new LogoutHandler(fx.Db).Handle(principal, CancellationToken.None);
        var (logoutStatus, _) = ResultAssert.Read<object>(logout);
        Assert.Equal(StatusCodes.Status204NoContent, logoutStatus);

        Assert.All(
            await fx.Db.RefreshSessions.AsNoTracking().ToListAsync(),
            session => Assert.NotNull(session.RevokedAt));

        var refresh = await new RefreshHandler(fx.Db, fx.Tokens)
            .Handle(new RefreshRequest(loginBody!.RefreshToken), CancellationToken.None);
        var (status, _) = ResultAssert.Read<ErrorResponse>(refresh);
        Assert.Equal(StatusCodes.Status401Unauthorized, status);
    }

    [Fact]
    public async Task Refresh_rejects_inactive_user()
    {
        await using var fx = new AuthTestFixture();
        var user = await fx.SeedUserAsync("user@example.com", "Secret1");

        var login = await new LoginHandler(fx.Db, fx.Passwords, fx.Tokens)
            .Handle(new LoginRequest("user@example.com", "Secret1"), CancellationToken.None);
        var (_, loginBody) = ResultAssert.Read<TokenResponse>(login);

        user.IsActive = false;
        await fx.Db.SaveChangesAsync();

        var refresh = await new RefreshHandler(fx.Db, fx.Tokens)
            .Handle(new RefreshRequest(loginBody!.RefreshToken), CancellationToken.None);
        var (status, _) = ResultAssert.Read<ErrorResponse>(refresh);
        Assert.Equal(StatusCodes.Status401Unauthorized, status);
    }
}

using System.Security.Claims;
using Investigacion1_back.Features.Auth.AdminRegister;
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;

namespace Investigacion1_back.Tests.Auth;

public class AdminRegisterHandlerTests
{
    [Fact]
    public async Task AdminRegister_allows_first_admin_without_auth()
    {
        await using var fx = new AuthTestFixture();
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await new AdminRegisterHandler(fx.Db, fx.Passwords)
            .Handle(new AdminRegisterRequest("admin@example.com", "Admin1a"), anonymous, CancellationToken.None);

        var (status, body) = ResultAssert.Read<AdminRegisterResponse>(result);
        Assert.Equal(StatusCodes.Status201Created, status);
        Assert.Equal(Roles.Admin, body!.Role);
    }

    [Fact]
    public async Task AdminRegister_requires_admin_when_admin_already_exists()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("admin@example.com", "Admin1a", role: Roles.Admin);
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await new AdminRegisterHandler(fx.Db, fx.Passwords)
            .Handle(new AdminRegisterRequest("admin2@example.com", "Admin1a"), anonymous, CancellationToken.None);

        var (status, _) = ResultAssert.Read<ErrorResponse>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, status);
    }

    [Fact]
    public async Task AdminRegister_allows_authenticated_admin_to_create_another()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("admin@example.com", "Admin1a", role: Roles.Admin);
        var admin = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, Roles.Admin)
        ], authenticationType: "Bearer"));

        var result = await new AdminRegisterHandler(fx.Db, fx.Passwords)
            .Handle(new AdminRegisterRequest("admin2@example.com", "Admin1a"), admin, CancellationToken.None);

        var (status, body) = ResultAssert.Read<AdminRegisterResponse>(result);
        Assert.Equal(StatusCodes.Status201Created, status);
        Assert.Equal("admin2@example.com", body!.Email);
    }
}

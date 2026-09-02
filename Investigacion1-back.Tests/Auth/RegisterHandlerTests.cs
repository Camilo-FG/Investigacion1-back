using Investigacion1_back.Features.Auth;
using Investigacion1_back.Features.Auth.Register;
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;

namespace Investigacion1_back.Tests.Auth;

public class RegisterHandlerTests
{
    [Fact]
    public async Task Register_creates_subscription_l1_user()
    {
        await using var fx = new AuthTestFixture();

        var result = await new RegisterHandler(fx.Db, fx.Passwords)
            .Handle(new RegisterRequest("new@example.com", "Secret1"), CancellationToken.None);

        var (status, body) = ResultAssert.Read<RegisterResponse>(result);
        Assert.Equal(StatusCodes.Status201Created, status);
        Assert.Equal("new@example.com", body!.Email);
        Assert.Equal(Roles.SubscriptionL1, body.Role);
        Assert.True(body.IsActive);
        Assert.True(fx.Passwords.Verify("Secret1", fx.Db.Users.Single().PasswordHash));
    }

    [Fact]
    public async Task Register_rejects_weak_password()
    {
        await using var fx = new AuthTestFixture();

        var result = await new RegisterHandler(fx.Db, fx.Passwords)
            .Handle(new RegisterRequest("new@example.com", "short"), CancellationToken.None);

        var (status, body) = ResultAssert.Read<ErrorResponse>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal(PasswordPolicy.ErrorMessage, body!.Error);
    }

    [Fact]
    public async Task Register_rejects_duplicate_email()
    {
        await using var fx = new AuthTestFixture();
        await fx.SeedUserAsync("new@example.com", "Secret1");

        var result = await new RegisterHandler(fx.Db, fx.Passwords)
            .Handle(new RegisterRequest("new@example.com", "Secret1"), CancellationToken.None);

        var (status, body) = ResultAssert.Read<ErrorResponse>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.Equal("Email is already registered.", body!.Error);
    }
}

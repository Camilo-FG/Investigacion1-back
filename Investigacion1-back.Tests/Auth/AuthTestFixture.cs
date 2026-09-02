using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Investigacion1_back.Tests.Auth;

internal sealed class AuthTestFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Db { get; }
    public PasswordService Passwords { get; } = new();
    public JwtTokenService Tokens { get; }

    public AuthTestFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();

        Tokens = new JwtTokenService(Options.Create(new JwtOptions
        {
            Issuer = "Investigacion1",
            Audience = "Investigacion1",
            Secret = "test-jwt-secret-at-least-32-chars-long!!",
            AccessTokenMinutes = 60,
            RefreshTokenDays = 14
        }));
    }

    public async Task<User> SeedUserAsync(
        string email,
        string password,
        string role = Roles.SubscriptionL1,
        bool isActive = true,
        DateTime? subscriptionExpiration = null)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = Passwords.Hash(password),
            Role = role,
            IsActive = isActive,
            SubscriptionExpirationDate = subscriptionExpiration ?? DateTime.UtcNow.AddYears(1)
        };

        Db.Users.Add(user);
        await Db.SaveChangesAsync();
        return user;
    }

    public ValueTask DisposeAsync()
    {
        Db.Dispose();
        _connection.Dispose();
        return ValueTask.CompletedTask;
    }
}

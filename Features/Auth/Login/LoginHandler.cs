using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Auth.Login;

public sealed class LoginHandler
{
    private readonly AppDbContext _db;
    private readonly PasswordService _passwords;
    private readonly JwtTokenService _tokens;

    public LoginHandler(AppDbContext db, PasswordService passwords, JwtTokenService tokens)
    {
        _db = db;
        _passwords = passwords;
        _tokens = tokens;
    }

    public async Task<IResult> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        var user = string.IsNullOrWhiteSpace(email)
            ? null
            : await _db.Users.FirstOrDefaultAsync(candidate => candidate.Email == email, cancellationToken);

        if (user is null
            || !_passwords.Verify(request.Password ?? string.Empty, user.PasswordHash)
            || !user.IsActive)
        {
            return Results.Json(
                new ErrorResponse("Invalid credentials."),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var accessToken = _tokens.CreateAccessToken(user);
        var (refreshToken, tokenHash, expiresAt) = _tokens.CreateRefreshToken();

        _db.RefreshSessions.Add(new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Ok(new TokenResponse(accessToken, refreshToken));
    }
}

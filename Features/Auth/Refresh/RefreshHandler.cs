using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Auth.Refresh;

public sealed class RefreshHandler
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _tokens;

    public RefreshHandler(AppDbContext db, JwtTokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    public async Task<IResult> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return InvalidRefreshToken();
        }

        var tokenHash = _tokens.HashRefreshToken(request.RefreshToken);
        var session = await _db.RefreshSessions
            .Include(candidate => candidate.User)
            .FirstOrDefaultAsync(candidate => candidate.TokenHash == tokenHash, cancellationToken);

        if (session is null || !session.IsUsable || !session.User.IsActive)
        {
            return InvalidRefreshToken();
        }

        session.RevokedAt = DateTime.UtcNow;

        var accessToken = _tokens.CreateAccessToken(session.User);
        var (refreshToken, newHash, expiresAt) = _tokens.CreateRefreshToken();

        _db.RefreshSessions.Add(new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = session.UserId,
            TokenHash = newHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Ok(new TokenResponse(accessToken, refreshToken));
    }

    private static IResult InvalidRefreshToken() =>
        Results.Json(
            new ErrorResponse("Invalid refresh token."),
            statusCode: StatusCodes.Status401Unauthorized);
}

using System.Security.Claims;
using Investigacion1_back.Shared.Auth;
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Auth.Logout;

public sealed class LogoutHandler
{
    private readonly AppDbContext _db;

    public LogoutHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(ClaimsPrincipal caller, CancellationToken cancellationToken)
    {
        var userId = caller.GetUserId();
        if (userId is null)
        {
            return Results.Json(
                new ErrorResponse("Authentication is required."),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var now = DateTime.UtcNow;
        var sessions = await _db.RefreshSessions
            .Where(session => session.UserId == userId && session.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.RevokedAt = now;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}

using System.Security.Claims;
using Investigacion1_back.Shared.Auth;
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Users.GetMe;

public sealed class GetMeHandler
{
    private readonly AppDbContext _db;

    public GetMeHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetCurrentUserQuery query, ClaimsPrincipal caller, CancellationToken cancellationToken)
    {
        var userId = caller.GetUserId();
        if (userId is null)
        {
            return Results.Json(
                new ErrorResponse("Authentication is required."),
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound(new ErrorResponse("Authenticated user was not found."));
        }

        return Results.Ok(UserResponse.From(user));
    }
}
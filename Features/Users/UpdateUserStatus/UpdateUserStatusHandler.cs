using System.Security.Claims;
using Investigacion1_back.Shared.Auth;
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Users.UpdateUserStatus;

public sealed class UpdateUserStatusHandler
{
    private readonly AppDbContext _db;

    public UpdateUserStatusHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(
        UpdateUserStatusCommand command,
        ClaimsPrincipal caller,
        CancellationToken cancellationToken)
    {
        var target = await _db.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == command.UserId, cancellationToken);

        if (target is null)
        {
            return Results.NotFound(new ErrorResponse("User not found."));
        }

        if (!command.IsActive)
        {
            if (caller.GetUserId() == target.Id)
            {
                return Forbidden("An Admin cannot deactivate their own account.");
            }

            if (target.Role == Roles.Admin && await IsLastActiveAdmin(cancellationToken))
            {
                return Forbidden("Cannot deactivate the last active Admin.");
            }
        }

        target.IsActive = command.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Ok(UserResponse.From(target));
    }

    private async Task<bool> IsLastActiveAdmin(CancellationToken cancellationToken)
    {
        var activeAdminCount = await _db.Users
            .CountAsync(user => user.Role == Roles.Admin && user.IsActive, cancellationToken);
        return activeAdminCount <= 1;
    }

    private static IResult Forbidden(string message) =>
        Results.Json(
            new ErrorResponse(message),
            statusCode: StatusCodes.Status403Forbidden);
}
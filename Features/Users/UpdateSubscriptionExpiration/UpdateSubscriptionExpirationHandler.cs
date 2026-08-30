using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Users.UpdateSubscriptionExpiration;

public sealed class UpdateSubscriptionExpirationHandler
{
    private readonly AppDbContext _db;

    public UpdateSubscriptionExpirationHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(
        UpdateSubscriptionExpirationCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound(new ErrorResponse("User not found."));
        }

        if (user.Role != Roles.SubscriptionL1)
        {
            return Results.BadRequest(new ErrorResponse(
                "Subscription expiration can only be updated for Subscription_L1 users."));
        }

        user.SubscriptionExpirationDate = command.NewExpirationDate.ToUniversalTime();
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Ok(UserResponse.From(user));
    }
}
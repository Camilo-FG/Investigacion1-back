using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Users.GetUserById;

public sealed class GetUserByIdHandler
{
    private readonly AppDbContext _db;

    public GetUserByIdHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound(new ErrorResponse("User not found."));
        }

        return Results.Ok(UserResponse.From(user));
    }
}
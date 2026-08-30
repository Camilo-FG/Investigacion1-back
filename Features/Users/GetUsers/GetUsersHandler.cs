using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Users.GetUsers;

public sealed class GetUsersHandler
{
    private readonly AppDbContext _db;

    public GetUsersHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _db.Users
            .AsNoTracking()
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        return Results.Ok(users.Select(UserResponse.From));
    }
}
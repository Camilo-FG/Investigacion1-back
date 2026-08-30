using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Rooms.GetRooms;

public sealed class GetRoomsHandler
{
    private readonly AppDbContext _db;

    public GetRoomsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(GetRoomsQuery query, CancellationToken cancellationToken)
    {
        var rooms = await _db.Rooms
            .AsNoTracking()
            .OrderBy(candidate => candidate.Number)
            .ToListAsync(cancellationToken);

        return Results.Ok(rooms.Select(RoomResponse.From));
    }
}
using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler
{
    private readonly AppDbContext _db;

    public CreateRoomHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var number = command.Number?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(number))
        {
            return Results.BadRequest(new ErrorResponse("Room number is required."));
        }

        if (command.BasePricePerNight <= 0)
        {
            return Results.BadRequest(new ErrorResponse("BasePricePerNight must be greater than zero."));
        }

        if (await _db.Rooms.AnyAsync(candidate => candidate.Number == number, cancellationToken))
        {
            return Results.BadRequest(new ErrorResponse("A room with that number already exists."));
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Number = number,
            Type = command.Type,
            Floor = command.Floor,
            Capacity = command.Capacity,
            BasePricePerNight = command.BasePricePerNight
        };

        _db.Rooms.Add(room);
        await _db.SaveChangesAsync(cancellationToken);

        return Results.Json(
            RoomResponse.From(room),
            statusCode: StatusCodes.Status201Created);
    }
}
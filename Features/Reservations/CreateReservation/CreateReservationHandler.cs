using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Features.Reservations.CreateReservation;

public sealed class CreateReservationHandler
{
    private readonly AppDbContext _db;

    public CreateReservationHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var guestName = command.GuestName?.Trim();
        if (string.IsNullOrWhiteSpace(guestName))
        {
            return Results.BadRequest(new ErrorResponse("GuestName is required."));
        }

        var room = await _db.Rooms
            .FirstOrDefaultAsync(candidate => candidate.Id == command.RoomId, cancellationToken);

        if (room is null)
        {
            return Results.NotFound(new ErrorResponse("Room not found."));
        }

        var checkIn = command.CheckInDate.Date;
        var checkOut = command.CheckOutDate.Date;

        if (checkOut <= checkIn)
        {
            return Results.BadRequest(new ErrorResponse("CheckOutDate must be after CheckInDate."));
        }

        if (command.Guests < 1 || command.Guests > room.Capacity)
        {
            return Results.BadRequest(new ErrorResponse(
                $"Guests must be between 1 and the room capacity ({room.Capacity})."));
        }

        var overlaps = await _db.Reservations.AnyAsync(
            candidate => candidate.RoomId == command.RoomId
                && candidate.CheckInDate < checkOut
                && candidate.CheckOutDate > checkIn,
            cancellationToken);

        if (overlaps)
        {
            return Results.Json(
                new ErrorResponse("Room is already reserved for that period."),
                statusCode: StatusCodes.Status409Conflict);
        }

        var nights = (int)(checkOut - checkIn).TotalDays;

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            GuestName = guestName,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            Guests = command.Guests,
            TotalPrice = nights * room.BasePricePerNight
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync(cancellationToken);

        reservation.Room = room;

        return Results.Json(
            ReservationResponse.From(reservation),
            statusCode: StatusCodes.Status201Created);
    }
}
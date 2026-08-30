namespace Investigacion1_back.Features.Reservations.CreateReservation;

public sealed record CreateReservationCommand(
    Guid RoomId,
    string GuestName,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int Guests);
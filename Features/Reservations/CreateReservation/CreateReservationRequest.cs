namespace Investigacion1_back.Features.Reservations.CreateReservation;

public sealed record CreateReservationRequest(
    Guid RoomId,
    string GuestName,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int Guests);
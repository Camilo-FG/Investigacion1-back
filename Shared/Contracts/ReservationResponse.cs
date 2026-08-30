using Investigacion1_back.Shared.Domain;

namespace Investigacion1_back.Shared.Contracts;

public sealed record ReservationResponse(
    Guid Id,
    Guid RoomId,
    string RoomNumber,
    string RoomType,
    int RoomFloor,
    int RoomCapacity,
    decimal BasePricePerNight,
    string GuestName,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int Guests,
    decimal TotalPrice)
{
    public static ReservationResponse From(Reservation reservation) =>
        new(
            reservation.Id,
            reservation.RoomId,
            reservation.Room.Number,
            reservation.Room.Type.ToString(),
            reservation.Room.Floor,
            reservation.Room.Capacity,
            reservation.Room.BasePricePerNight,
            reservation.GuestName,
            reservation.CheckInDate,
            reservation.CheckOutDate,
            reservation.Guests,
            reservation.TotalPrice);
}
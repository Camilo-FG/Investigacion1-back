using Investigacion1_back.Shared.Domain;

namespace Investigacion1_back.Shared.Contracts;

public sealed record RoomResponse(
    Guid Id,
    string Number,
    string Type,
    int Floor,
    int Capacity,
    decimal BasePricePerNight)
{
    public static RoomResponse From(Room room) =>
        new(room.Id, room.Number, room.Type.ToString(), room.Floor, room.Capacity, room.BasePricePerNight);
}
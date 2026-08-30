using Investigacion1_back.Shared.Domain;

namespace Investigacion1_back.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(
    string Number,
    RoomType Type,
    int Floor,
    int Capacity,
    decimal BasePricePerNight);
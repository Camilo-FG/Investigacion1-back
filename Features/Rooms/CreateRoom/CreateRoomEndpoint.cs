using Investigacion1_back.Shared.Auth;

namespace Investigacion1_back.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/rooms", async (
            CreateRoomRequest request,
            CreateRoomHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(
                new CreateRoomCommand(
                    request.Number,
                    request.Type,
                    request.Floor,
                    request.Capacity,
                    request.BasePricePerNight),
                cancellationToken))
            .RequireAuthorization(AuthPolicies.AdminOnly);
    }
}
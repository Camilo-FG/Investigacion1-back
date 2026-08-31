namespace Investigacion1_back.Features.Rooms.GetRooms;

public static class GetRoomsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/rooms", async (
            GetRoomsHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(new GetRoomsQuery(), cancellationToken))
            .RequireAuthorization();
    }
}
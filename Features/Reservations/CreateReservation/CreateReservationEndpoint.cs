namespace Investigacion1_back.Features.Reservations.CreateReservation;

public static class CreateReservationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/reservations", async (
            CreateReservationRequest request,
            CreateReservationHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(
                new CreateReservationCommand(
                    request.RoomId,
                    request.GuestName,
                    request.CheckInDate,
                    request.CheckOutDate,
                    request.Guests),
                cancellationToken))
            .RequireAuthorization();
    }
}
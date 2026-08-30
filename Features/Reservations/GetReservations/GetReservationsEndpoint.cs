using Investigacion1_back.Shared.Auth;

namespace Investigacion1_back.Features.Reservations.GetReservations;

public static class GetReservationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/reservations", async (
            GetReservationsHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(new GetReservationsQuery(), cancellationToken))
            .RequireAuthorization(AuthPolicies.AdminOnly);
    }
}
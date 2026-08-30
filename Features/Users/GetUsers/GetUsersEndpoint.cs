using Investigacion1_back.Shared.Auth;

namespace Investigacion1_back.Features.Users.GetUsers;

public static class GetUsersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (
            GetUsersHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(new GetUsersQuery(), cancellationToken))
            .RequireAuthorization(AuthPolicies.AdminOnly);
    }
}
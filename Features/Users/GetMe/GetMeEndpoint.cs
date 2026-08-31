using System.Security.Claims;

namespace Investigacion1_back.Features.Users.GetMe;

public static class GetMeEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", async (
            ClaimsPrincipal caller,
            GetMeHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(new GetCurrentUserQuery(), caller, cancellationToken))
            .RequireAuthorization();
    }
}
using System.Security.Claims;

namespace Investigacion1_back.Features.Auth.Logout;

public static class LogoutEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/logout", async (
            ClaimsPrincipal user,
            LogoutHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(user, cancellationToken))
            .RequireAuthorization();
    }
}

using System.Security.Claims;

namespace Investigacion1_back.Features.Auth.AdminRegister;

public static class AdminRegisterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/register", async (
            AdminRegisterRequest request,
            ClaimsPrincipal user,
            AdminRegisterHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(request, user, cancellationToken));
    }
}

namespace Investigacion1_back.Features.Auth.Login;

public static class LoginEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            LoginRequest request,
            LoginHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(request, cancellationToken));
    }
}

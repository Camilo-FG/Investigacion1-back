namespace Investigacion1_back.Features.Auth.Register;

public static class RegisterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (
            RegisterRequest request,
            RegisterHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(request, cancellationToken));
    }
}

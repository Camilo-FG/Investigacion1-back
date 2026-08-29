namespace Investigacion1_back.Features.Auth.Refresh;

public static class RefreshEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/refresh", async (
            RefreshRequest request,
            RefreshHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(request, cancellationToken));
    }
}

using Investigacion1_back.Shared.Auth;

namespace Investigacion1_back.Features.Users.GetUserById;

public static class GetUserByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}", async (
            Guid id,
            GetUserByIdHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(new GetUserByIdQuery(id), cancellationToken))
            .RequireAuthorization(AuthPolicies.AdminOnly);
    }
}
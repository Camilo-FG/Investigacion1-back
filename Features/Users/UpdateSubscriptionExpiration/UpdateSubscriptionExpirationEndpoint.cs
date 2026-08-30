using Investigacion1_back.Shared.Auth;

namespace Investigacion1_back.Features.Users.UpdateSubscriptionExpiration;

public static class UpdateSubscriptionExpirationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/{id:guid}/subscription-expiration", async (
            Guid id,
            UpdateSubscriptionExpirationRequest request,
            UpdateSubscriptionExpirationHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(
                new UpdateSubscriptionExpirationCommand(id, request.SubscriptionExpirationDate),
                cancellationToken))
            .RequireAuthorization(AuthPolicies.AdminOnly);
    }
}
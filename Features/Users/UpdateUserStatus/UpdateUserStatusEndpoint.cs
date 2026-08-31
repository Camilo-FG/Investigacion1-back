using System.Security.Claims;
using Investigacion1_back.Shared.Auth;

namespace Investigacion1_back.Features.Users.UpdateUserStatus;

public static class UpdateUserStatusEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPatch("/users/{id:guid}/status", async (
            Guid id,
            UpdateUserStatusRequest request,
            ClaimsPrincipal caller,
            UpdateUserStatusHandler handler,
            CancellationToken cancellationToken) =>
            await handler.Handle(
                new UpdateUserStatusCommand(id, request.IsActive),
                caller,
                cancellationToken))
            .RequireAuthorization(AuthPolicies.AdminOnly);
    }
}
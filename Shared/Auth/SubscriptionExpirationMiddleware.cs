using Investigacion1_back.Shared.Contracts;
using Investigacion1_back.Shared.Domain;
using Investigacion1_back.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Investigacion1_back.Shared.Auth;

public sealed class SubscriptionExpirationMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var principal = context.User;
        if (principal.Identity?.IsAuthenticated == true
            && principal.GetRole() == Roles.SubscriptionL1)
        {
            var userId = principal.GetUserId();
            var expiration = await db.Users
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => (DateTime?)user.SubscriptionExpirationDate)
                .FirstOrDefaultAsync(context.RequestAborted);

            if (expiration is not null && expiration.Value < DateTime.UtcNow)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(
                    new ErrorResponse("Subscription has expired."),
                    context.RequestAborted);
                return;
            }
        }

        await _next(context);
    }
}
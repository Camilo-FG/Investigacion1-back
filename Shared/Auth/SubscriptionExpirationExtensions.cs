namespace Investigacion1_back.Shared.Auth;

public static class SubscriptionExpirationExtensions
{
    public static IApplicationBuilder UseSubscriptionExpirationCheck(this IApplicationBuilder app) =>
        app.UseMiddleware<SubscriptionExpirationMiddleware>();
}
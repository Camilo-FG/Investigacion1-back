using Investigacion1_back.Shared.Domain;

namespace Investigacion1_back.Shared.Contracts;

public sealed record UserResponse(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    DateTime SubscriptionExpirationDate)
{
    public static UserResponse From(User user) =>
        new(user.Id, user.Email, user.Role, user.IsActive, user.SubscriptionExpirationDate);
}
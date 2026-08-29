namespace Investigacion1_back.Features.Auth.Register;

public sealed record RegisterResponse(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    DateTime SubscriptionExpirationDate);

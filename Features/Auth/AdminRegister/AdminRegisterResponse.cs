namespace Investigacion1_back.Features.Auth.AdminRegister;

public sealed record AdminRegisterResponse(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    DateTime SubscriptionExpirationDate);

namespace Investigacion1_back.Features.Users.UpdateUserStatus;

public sealed record UpdateUserStatusCommand(Guid UserId, bool IsActive);
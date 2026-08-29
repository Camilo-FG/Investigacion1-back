namespace Investigacion1_back.Shared.Domain;

public class RefreshSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public bool IsUsable => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}

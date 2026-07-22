namespace Pingr.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    
    public string TokenHash { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    
    // Required by EF Core
    private RefreshToken() { }

    public RefreshToken(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = expiresAt;
    }

    public void Revoke() 
        => RevokedAt = DateTimeOffset.UtcNow;
}
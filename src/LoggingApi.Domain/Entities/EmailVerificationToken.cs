namespace LoggingApi.Domain.Entities;

/// <summary>
/// Represents a user's email verification token.
/// </summary>
public sealed class EmailVerificationToken
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    
    public string TokenHash { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset ExpiresAt { get; private set; }
    
    // Required by EF Core
    private EmailVerificationToken() { }

    public EmailVerificationToken(
        Guid userId,
        User user,
        string tokenHash)
    {
        UserId = userId;
        User = user;
        TokenHash = tokenHash;
        CreatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = DateTimeOffset.UtcNow.AddHours(24);
    }
}
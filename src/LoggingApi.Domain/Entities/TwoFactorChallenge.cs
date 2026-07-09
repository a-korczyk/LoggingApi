namespace LoggingApi.Domain.Entities;

/// <summary>
/// Represents a user's two factor challenge.
/// </summary>
public sealed class TwoFactorChallenge
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public string TokenHash { get; private set; }

    public TwoFactorChallengePurpose TwoFactorChallengePurpose { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    // Required by EF Core
    private TwoFactorChallenge() { }

    public TwoFactorChallenge(
        Guid userId,
        User user,
        string tokenHash,
        TwoFactorChallengePurpose twoFactorChallengePurpose)
    {
        UserId = userId;
        User = user;
        TokenHash = tokenHash;
        TwoFactorChallengePurpose = twoFactorChallengePurpose;
        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
    }
}

public enum TwoFactorChallengePurpose
{
    Confirm2FaSetup = 0,
    Login = 1,
}
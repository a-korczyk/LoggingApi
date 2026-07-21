using LoggingApi.Domain.Common;

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
        string tokenHash,
        TwoFactorChallengePurpose twoFactorChallengePurpose)
    {
        UserId = userId;
        TokenHash = tokenHash;
        TwoFactorChallengePurpose = twoFactorChallengePurpose;
        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
    }

    public void Update(
        string newTokenHash,
        TwoFactorChallengePurpose newPurpose)
    {
        TokenHash = newTokenHash;
        TwoFactorChallengePurpose = newPurpose;
        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10);
    }
    
    /// <summary>
    /// Checks if the provided challenge is valid.
    /// </summary>
    /// <returns>Challenge if valid, otherwise an error describing
    /// why it isn't valid.</returns>
    public static Result<TwoFactorChallenge> ValidateChallenge(
        TwoFactorChallenge? challenge,
        TwoFactorChallengePurpose expectedPurpose,
        Guid? expectedUserId = null,
        string? expectedTokenHash = null)
    {
        if (challenge is null)
            return TwoFactorErrors.NoChallengeFound;

        if (expectedUserId is not null && challenge.UserId != expectedUserId)
            return TwoFactorErrors.NoChallengeFound;
        
        if (expectedTokenHash is not null && challenge.TokenHash != expectedTokenHash)
            return TwoFactorErrors.InvalidToken;
        
        if (challenge.TwoFactorChallengePurpose != expectedPurpose)
            return TwoFactorErrors.NoChallengeFound;
        
        if (challenge.ExpiresAt < DateTimeOffset.UtcNow)
            return TwoFactorErrors.ExpiredChallenge;

        return challenge;
    }
}

public enum TwoFactorChallengePurpose
{
    Confirm2FaSetup = 0,
    Login = 1,
    DeleteAccount = 2,
    DeleteWorkspace = 3
}
using System.Collections.Immutable;

namespace LoggingApi.Domain.Entities;

/// <summary>
/// Represents an application user.
/// </summary>
public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool EmailConfirmed { get; private set; }
    
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }
    public IList<string>? TwoFactorRecoveryCodes { get; private set; }
    
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    public ICollection<WorkspaceUser> WorkspaceUsers { get; private set; } = new List<WorkspaceUser>();
    public ICollection<Workspace> OwnedWorkspaces { get; private set; } = new List<Workspace>();
    
    // Required by EF Core
    private User() { }

    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        EmailConfirmed = false;
        TwoFactorEnabled = false;
    }
    
    public void ConfirmEmail() => EmailConfirmed = true;
    
    public void EnableTwoFactor() => TwoFactorEnabled = true;
    
    public void AddTwoFactorSecret(string secret) => TwoFactorSecret = secret;
    
    public void AddTwoFactorRecoveryCodes(IList<string> recoveryCodes) 
        => TwoFactorRecoveryCodes = recoveryCodes;
    public void RemoveTwoFactorRecoveryCode(string recoveryCode)
        => TwoFactorRecoveryCodes?.Remove(recoveryCode);
}
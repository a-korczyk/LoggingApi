namespace LoggingApi.Domain.Entities;

/// <summary>
/// Represents an application user.
/// </summary>
public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    
    // Required by EF Core
    private User() { }

    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
    }
}
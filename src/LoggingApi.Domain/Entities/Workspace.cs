namespace LoggingApi.Domain.Entities;

/// <summary>
/// Represents a workspace that contains logs and it's users.
/// </summary>
public sealed class Workspace
{
    public Guid Id { get; private set; }
    
    public Guid OwnerUserId { get; private set; }
    public User OwnerUser { get; private set; }
    
    public ICollection<WorkspaceUser> WorkspaceUsers { get; private set; } = new List<WorkspaceUser>();
    
    public string Name { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    // Required by EF Core
    private Workspace() { }

    public Workspace(
        Guid ownerUserId,
        string name)
    {
        Id = Guid.NewGuid();
        
        OwnerUserId = ownerUserId;
        
        Name = name;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
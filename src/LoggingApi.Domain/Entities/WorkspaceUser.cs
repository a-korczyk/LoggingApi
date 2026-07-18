namespace LoggingApi.Domain.Entities;

/// <summary>
/// Represents a workspace user.
/// </summary>
public sealed class WorkspaceUser
{
   public Guid WorkspaceId { get; private set; }
   public Workspace Workspace { get; private set; }
   
   public Guid UserId { get; private set; }
   public User User { get; private set; }
   
   public WorkspaceRole Role { get; private set; }
   
   // Required by EF Core
   private WorkspaceUser() { }

   public WorkspaceUser(
       Guid workspaceId,
       Guid userId,
       WorkspaceRole role)
   {
       WorkspaceId = workspaceId;
       
       UserId = userId;
       
       Role = role;
   }
   
   public void UpdateRole(WorkspaceRole newRole) 
       => Role = newRole;
}

/// <summary>
/// Represents a user's role in a workspace that defines
/// what permissions they have.
/// </summary>
public enum WorkspaceRole
{
   User = 0,
   Admin = 1,
   Owner = 2
}
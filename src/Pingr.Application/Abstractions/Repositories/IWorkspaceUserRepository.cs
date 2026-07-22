using Pingr.Domain.Entities;

namespace Pingr.Application.Abstractions.Repositories;

/// <summary>
/// Provides methods for accessing and persisting <see cref="WorkspaceUser"/> entities.
/// </summary>
public interface IWorkspaceUserRepository
{
    /// <summary>
    /// Adds the provided <see cref="WorkspaceUser"/> to the current unit of work.
    /// </summary>
    Task AddAsync(
        WorkspaceUser workspaceUser,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific <see cref="WorkspaceUser"/> by it's
    /// user's identifier and workspace identifier.
    /// </summary>
    Task<WorkspaceUser?> GetAsync(
        Guid userId,
        Guid workspaceId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all <see cref="WorkspaceUser"/> entities by the provided user.
    /// </summary>
    /// <param name="pagination">The pagination settings.</param>
    Task<ICollection<WorkspaceUser>> GetByUserIdAsync(
        Guid userId,
        Pagination pagination,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves all <see cref="WorkspaceUser"/> entities by the provided workspace.
    /// </summary>
    /// <param name="pagination">The pagination settings.</param>
    Task<ICollection<WorkspaceUser>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Pagination pagination,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the user is a member of the workspace.
    /// </summary>
    Task<bool> IsMemberAsync(
        Guid userId,
        Guid workspaceId,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Checks if the user has one of the specified roles in a workspace.
    /// </summary>
    Task<bool> IsInRoleAsync(
        Guid userId,
        Guid workspaceId,
        ICollection<WorkspaceRole> roles,
        CancellationToken cancellationToken);

    /// <summary>
    /// Begins tracking the provided <see cref="WorkspaceUser"/> as deleted.
    /// </summary>
    void Delete(
        WorkspaceUser workspaceUser);
}
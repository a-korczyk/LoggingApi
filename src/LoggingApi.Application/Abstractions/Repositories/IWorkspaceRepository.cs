using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Repositories;

/// <summary>
/// Provides methods for accessing and persisting <see cref="Workspace"/> entities.
/// </summary>
public interface IWorkspaceRepository
{
    /// <summary>
    /// Adds the provided workspace to the current unit of work.
    /// </summary>
    Task AddAsync(
        Workspace workspace,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a workspace by its identifier.
    /// </summary>
    Task<Workspace?> GetByWorkspaceIdAsync(
        Guid workspaceId,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a workspace by its name.
    /// </summary>
    Task<Workspace?> GetByWorkspaceNameAsync(
        string workspaceName,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated collection of all workspaces a
    /// user is a member. 
    /// </summary>
    /// <returns></returns>
    Task<ICollection<Workspace>> GetByUserIdAsync(
        Guid userId,
        Pagination pagination,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all workspaces that are owned by a user.
    /// </summary>
    /// <param name="ownerUserId">The identifier of the owner.</param>
    /// <param name="pagination">The pagination settings.</param>
    Task<ICollection<Workspace>> GetByOwnerUserIdAsync(
        Guid ownerUserId,
        Pagination pagination,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Starts tracking the provided workspace as deleted.
    /// </summary>
    void Delete(Workspace workspace);
}
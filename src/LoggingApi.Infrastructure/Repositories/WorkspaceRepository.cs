using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingApi.Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class WorkspaceRepository(
    ApplicationDbContext dbContext) : IWorkspaceRepository
{
    public async Task AddAsync(
        Workspace workspace,
        CancellationToken cancellationToken)
    {
        await dbContext.Workspaces.AddAsync(workspace, cancellationToken);
    }
    
    public async Task<Workspace?> GetByWorkspaceIdAsync(
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Workspaces.FindAsync(
            workspaceId,
            cancellationToken);
    }

    public async Task<Workspace?> GetByWorkspaceNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await dbContext.Workspaces.FirstOrDefaultAsync(
            x => x.Name == name,
            cancellationToken);
    }

    public async Task<ICollection<Workspace>> GetByUserIdAsync(
        Guid userId,
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        return await dbContext.Workspaces
            .Where(x => x.WorkspaceUsers.Any(wu => wu.UserId == userId))
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<Workspace>> GetByOwnerUserIdAsync(
        Guid ownerUserId,
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        return await dbContext.Workspaces
            .Where(x => x.OwnerUserId == ownerUserId)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public void Delete(Workspace workspace)
    {
        dbContext.Workspaces.Remove(workspace);
    }
}
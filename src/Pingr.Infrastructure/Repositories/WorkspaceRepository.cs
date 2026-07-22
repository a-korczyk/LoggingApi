using Pingr.Application.Abstractions.Repositories;
using Pingr.Application.Abstractions.Services;
using Pingr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pingr.Infrastructure.Repositories;

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
            .OrderByDescending(x => x.Id)
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
            .OrderByDescending(x => x.OwnerUserId)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public void Delete(Workspace workspace)
    {
        dbContext.Workspaces.Remove(workspace);
    }
}
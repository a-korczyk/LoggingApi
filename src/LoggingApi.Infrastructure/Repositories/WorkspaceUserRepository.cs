using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingApi.Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class WorkspaceUserRepository(
    ApplicationDbContext dbContext) : IWorkspaceUserRepository
{
    public async Task AddAsync(
        WorkspaceUser workspaceUser,
        CancellationToken cancellationToken)
    {
        await dbContext.WorkspaceUsers.AddAsync(workspaceUser, cancellationToken);
    }

    public async Task<WorkspaceUser?> GetAsync(
        Guid userId,
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceUsers.FirstOrDefaultAsync(
            x => x.UserId == userId && x.WorkspaceId == workspaceId,
            cancellationToken);
    }
    
    public async Task<ICollection<WorkspaceUser>> GetByUserIdAsync(
        Guid userId,
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceUsers
            .Where(x => x.UserId == userId)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<WorkspaceUser>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceUsers
            .Where(x => x.WorkspaceId == workspaceId)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsMemberAsync(
        Guid userId,
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceUsers.AnyAsync(
            x => x.UserId == userId && x.WorkspaceId == workspaceId,
            cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(
        Guid userId,
        Guid workspaceId,
        WorkspaceRole role,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceUsers.AnyAsync(
            x => x.UserId == userId && x.WorkspaceId == workspaceId && x.Role == role,
            cancellationToken);
    }

    public void Delete(
        WorkspaceUser workspaceUser)
    {
        dbContext.WorkspaceUsers.Remove(workspaceUser);
    }
}
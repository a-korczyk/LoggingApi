using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Domain.Entities;
using LoggingApi.Infrastructure.Services;
using LoggingApi.Infrastructure.Services.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LoggingApi.Infrastructure.Repositories;

/// <summary>
/// Implementation of <see cref="IUserRepository"/>
/// </summary>
public sealed class UserRepository(
    ApplicationDbContext dbContext,
    ICacheService cacheService,
    IOptions<AccessTokenOptions> accessTokenOptions) : IUserRepository
{
    private readonly AccessTokenOptions _accessTokenOptions = accessTokenOptions.Value;
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FindAsync(id, cancellationToken);
    }
    
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(
            x => x.Email == email,
            cancellationToken);
    }
    
    public async Task<ICollection<User>> GetByWorkspaceId(
        Guid workspaceId,
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        return await dbContext.WorkspaceUsers
            .Where(x => x.WorkspaceId == workspaceId)
            .OrderByDescending(x => x.WorkspaceId)
            .Select(x => x.User)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Delete(
        User user)
    {
        dbContext.Users.Remove(user);
        cacheService.SetAsync(
            $"deleted:users:{user.Id}",
            true,
            TimeSpan.FromMinutes(_accessTokenOptions.ExpirationInMinutes));
    }
}
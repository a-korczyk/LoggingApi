using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ILogRepository"/>.
/// </summary>
public sealed class LogRepository(ApplicationDbContext dbContext) : ILogRepository
{
    public async Task AddAsync(
        Log log,
        CancellationToken cancellationToken)
    {
        await dbContext.Logs.AddAsync(log, cancellationToken);
    }

    public async Task<Log?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await dbContext.Logs.FindAsync(id, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Log>> GetAsync(
        Guid userId,
        Pagination pagination,
        CancellationToken cancellationToken)
    {
        return await dbContext.Logs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToArrayAsync(cancellationToken);
    }

    public void Delete(
        Log log)
    {
        dbContext.Logs.Remove(log);
    }
}
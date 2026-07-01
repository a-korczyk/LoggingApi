using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Features.Logs.Queries;
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
        LogFilters? filters,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Logs
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (filters is not null)
        {
            if (filters.Statuses is { Count: > 0})
                query = query.Where(x => filters.Statuses.Contains(x.Status));

            if (filters.Types is { Count: > 0})
                query = query.Where(x => filters.Types.Contains(x.Type));
        
            if (!string.IsNullOrWhiteSpace(filters.TitleContains))
                query = query.Where(x => x.Title.Contains(filters.TitleContains));
        
            if (filters.CreatedBefore is not null)
                query = query.Where(x => x.CreatedAt <= filters.CreatedBefore.Value);
        
            if (filters.CreatedAfter is not null)
                query = query.Where(x => x.CreatedAt >= filters.CreatedAfter.Value);
        }
        
        return await query
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
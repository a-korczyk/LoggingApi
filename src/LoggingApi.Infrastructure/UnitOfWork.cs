using LoggingApi.Application.Abstractions;

namespace LoggingApi.Infrastructure;

/// <summary>
/// Implementation of <see cref="IUnitOfWork"/>.
/// </summary>
public sealed class UnitOfWork(ApplicationDbContext dbContext)
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
namespace LoggingApi.Application.Abstractions;

/// <summary>
/// Defines a unit of work for persisting changes made during the current operation.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
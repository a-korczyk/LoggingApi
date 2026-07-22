using System.ComponentModel.DataAnnotations;
using LoggingApi.Application.Features.Logs.Queries;
using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Repositories;

/// <summary>
/// Provides methods for accessing and persisting <see cref="Log"/> entities.
/// </summary>
public interface ILogRepository
{
   /// <summary>
   /// Adds the specified log to the current unit of work.
   /// </summary>
   /// <param name="log">The log to add.</param>
   /// <param name="cancellationToken">The cancellation token.</param>
   Task AddAsync(Log log, CancellationToken cancellationToken);

   /// <summary>
   /// Retrieves a log based on its identifier.
   /// </summary>
   /// <param name="id">The identifier of the log to get.</param>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>The log if found; otherwise <c>null</c>.</returns>
   Task<Log?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

   /// <summary>
   /// Retrieves a paginated and filtered collection of logs associated with the specified user.
   /// </summary>
   /// <param name="userId">The identifier of the user whose logs to retrieve.</param>
   /// <param name="pagination">The pagination settings.</param>
   /// <param name="filters">The log filters.</param>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>A read-only collection of logs that belong to the specified user.</returns>
   /// <remarks>
   /// If there are no logs associated with the provided user, or the requested page
   /// contains no logs, then an empty collection is returned.
   /// </remarks>
   Task<IReadOnlyList<Log>> GetByWorkspaceIdAsync(
      Guid userId,
      Pagination pagination,
      LogFilters? filters,
      CancellationToken cancellationToken);
   
   /// <summary>
   /// Starts tracking the specified log as deleted.
   /// </summary>
   /// <param name="log">The log to track as deleted.</param>
   void Delete(Log log);
}

/// <summary>
/// Represents pagination settings for a paged query.
/// </summary>
/// <param name="Page">The page number to retrieve. The first page is 1.</param>
/// <param name="PageSize">The maximum number of items to return per page. The default value is 20.</param>
public sealed record Pagination(
   int Page,
   int PageSize)
{
   [Range(1, int.MaxValue)] 
   public const int DefaultPage = 1;
   
   [Range(1, 100)]
   public const int DefaultPageSize = 20;
};

/// <summary>
/// Represents the filters to be used to select logs.
/// </summary>
public sealed record LogFilters(
   IReadOnlyList<LogStatus>? Statuses,
   IReadOnlyList<LogType>? Types,
   string? TitleContains,
   DateTimeOffset? CreatedBefore,
   DateTimeOffset? CreatedAfter);

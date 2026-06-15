using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Repositories;

/// <summary>
/// Provides methods for accessing and persisting user data.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email to search for the user by.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The user if found; otherwise <c>null</c>.
    /// </returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    
    /// <summary>
    /// Inserts a new user.
    /// </summary>
    /// <param name="user">The user to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(User user, CancellationToken cancellationToken);
}   
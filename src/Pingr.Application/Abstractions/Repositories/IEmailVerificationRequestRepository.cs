using Pingr.Domain.Entities;

namespace Pingr.Application.Abstractions.Repositories;

/// <summary>
/// Provides methods for accessing and persisting email verification requests.
/// </summary>
public interface IEmailVerificationRequestRepository
{
    /// <summary>
    /// Persists the provided <see cref="EmailVerificationRequest"/>.
    /// </summary>
    /// <param name="emailVerificationRequest">The request to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AddAsync(
        EmailVerificationRequest emailVerificationRequest,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a log by its identifier.
    /// </summary>
    /// <param name="userId">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="EmailVerificationRequest"/> or null.</returns>
    public Task<EmailVerificationRequest?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the provided <see cref="EmailVerificationRequest"/>.
    /// </summary>
    /// <param name="emailVerificationRequest">The request to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task DeleteAsync(
        EmailVerificationRequest emailVerificationRequest,
        CancellationToken cancellationToken);
}
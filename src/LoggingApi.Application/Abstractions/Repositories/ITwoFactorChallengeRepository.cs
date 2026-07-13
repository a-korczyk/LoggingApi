using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Repositories;

/// <summary>
/// Provides persistence methods for two-factor authentication challenges.
/// </summary>
public interface ITwoFactorChallengeRepository
{
    /// <summary>
    /// Marks the provided challenge as added.
    /// </summary>
    public Task AddAsync(
        TwoFactorChallenge twoFactorChallenge,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves the challenge that matches the provided identifier.
    /// </summary>
    /// <param name="userId">Identifier of the challenge.</param>
    /// <returns>The challenge if found, otherwise null.</returns>
    public Task<TwoFactorChallenge?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves the challenge whose token hash matches the provided one.
    /// </summary>
    /// <param name="tokenHash">The token hash.</param>
    /// <returns>The challenge if found, otherwise null.</returns>
    public Task<TwoFactorChallenge?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    /// <summary>
    /// Marks the provided challenge as deleted.
    /// </summary>
    public void Delete(
        TwoFactorChallenge twoFactorChallenge);
}
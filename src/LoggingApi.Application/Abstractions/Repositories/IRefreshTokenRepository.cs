using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Repositories;

/// <summary>
/// Provides persistence methods for refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Marks the provided refresh token as added.
    /// </summary>
    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves a refresh token that belongs to the provided
    /// user and contains the provided token hash. 
    /// </summary>
    Task<RefreshToken?> GetAsync(
        Guid userId,
        string tokenHash,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves all refresh tokens belonging to a user.
    /// </summary>
    Task<ICollection<RefreshToken>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Services;

/// <summary>
/// Provides methods for generating and hashing email verification tokens.
/// </summary>
public interface IEmailVerificationTokenProvider
{
    /// <summary>
    /// Returns a cryptographically secure and URL-safe token.
    /// </summary>
    public string GenerateToken();
    
    /// <summary>
    /// Cryptographically hashes the provided token for safe storage.
    /// </summary>
    /// <param name="token">The token to hash.</param>
    /// <returns>The hashed token.</returns>
    public string HashToken(string token);
}
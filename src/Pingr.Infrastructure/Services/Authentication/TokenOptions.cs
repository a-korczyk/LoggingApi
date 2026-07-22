using System.ComponentModel.DataAnnotations;

namespace Pingr.Infrastructure.Services.Authentication;

/// <summary>
/// Configuration options for signing and issuing JWT tokens.
/// Bound from the <c>Tokens:AccessToken</c> configuration section.
/// </summary>
public sealed class AccessTokenOptions 
{
    public const string SectionName = "Tokens:AccessToken";
    
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    
    /// <summary>
    /// JWT symmetric signing key.
    /// Must be at least 32 characters long.
    /// </summary>
    [MinLength(32)]
    [DeniedValues("NOT_ACTUAL_SECRET_CHANGE_ME_BEFORE_PRODUCTION_32_CHAR_SECRET")]
    public required string Secret { get; init; }
    
    [Range(1, 1440)]
    public required int ExpirationInMinutes { get; init; }
}

/// <summary>
/// Configuration options for refresh tokens.
/// Bound from the <c>Tokens:RefreshToken</c> configuration section.
/// </summary>
public sealed class RefreshTokenOptions
{
    public const string SectionName = "Tokens:RefreshToken";
    
    [Range(1, 365)]
    public required int ExpirationInDays { get; init; }
}
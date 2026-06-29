using System.ComponentModel.DataAnnotations;

namespace LoggingApi.Contracts;

/// <summary>
/// Request sent to authenticate a user.
/// </summary>
public sealed record LoginRequest(
    [Required]
    string Email,
    
    [Required]
    string Password);

/// <summary>
/// Response returned after a successful login.
/// </summary>
public sealed record LoginResponse(
    string JwtToken);


/// <summary>
/// Request sent to register a new user.
/// </summary>
public sealed record RegisterRequest(
    [Required]
    string Email,
    
    [Required]
    string Password);

/// <summary>
/// Response returned after a successful registration.
/// </summary>
public sealed record RegisterResponse(
    string JwtToken);

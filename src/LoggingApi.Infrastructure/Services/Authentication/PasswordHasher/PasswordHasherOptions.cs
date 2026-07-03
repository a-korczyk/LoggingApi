using System.Security.Cryptography;
using LoggingApi.Application.Abstractions.Services;

namespace LoggingApi.Infrastructure.Services.Authentication.PasswordHasher;

/// <summary>
/// Configuration options for <see cref="IPasswordHasher"/>
/// </summary>
public sealed class PasswordHasherOptions
{
    public int SaltSize { get; init; } = 16;
    public int Iterations { get; init; } = 100_000;
    public HashAlgorithmName HashAlgorithmName { get; init; } = HashAlgorithmName.SHA512;
    public int HashSize { get; init; } = 32;
}
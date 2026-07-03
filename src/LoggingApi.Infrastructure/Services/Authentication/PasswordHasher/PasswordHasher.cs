using System.Security.Cryptography;
using LoggingApi.Application.Abstractions.Services;
using Microsoft.Extensions.Options;

namespace LoggingApi.Infrastructure.Services.Authentication.PasswordHasher;

/// <summary>
/// Implementation of <see cref="IPasswordHasher"/>
/// </summary>
public class PasswordHasher(IOptions<PasswordHasherOptions> options) : IPasswordHasher
{
    private readonly PasswordHasherOptions _options = options.Value;

    public string HashPassword(string password, CancellationToken cancellationToken)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(_options.SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _options.Iterations, _options.HashAlgorithmName, _options.HashSize);
        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}-{_options.Iterations}-{_options.HashAlgorithmName}-{_options.HashSize}";
    }

    public bool VerifyPassword(string inputPassword, string hashedPassword, CancellationToken cancellationToken)
    {
        string[] parts = hashedPassword.Split("-");
        
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);
        int iterations = Convert.ToInt32(parts[2]);
        string hashAlgorithmName = parts[3];
        int hashSize = Convert.ToInt32(parts[4]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, iterations, new HashAlgorithmName(hashAlgorithmName), hashSize);
        
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}
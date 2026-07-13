namespace LoggingApi.Application.Abstractions.Services;

/// <summary>
/// Provides two-factor authentication related methods.
/// </summary>
public interface ITwoFactorService
{
    public byte[] GenerateSecret();
    
    public string EncodeSecret(byte[] secret);
    
    public ISet<string> GenerateRecoveryCodes();
    
    public ISet<string> HashRecoveryCodes(IEnumerable<string> recoveryCodes);

    public string GenerateQrCode(string userEmail,
        byte[] userTwoFactorSecret);

    public bool VerifyTotpCode(
        string totpCode,
        string userTwoFactorSecret);
}
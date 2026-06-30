using Microsoft.JSInterop;

namespace LoggingApi.Blazor.Services.Authentication;

/// <summary>
/// Provides methods for persisting JWT tokens.
/// </summary>
public interface ITokenStore
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task RemoveTokenAsync();
}

/// <inheritdoc />
public sealed class TokenStore(
    ILocalStorageService localStorage) : ITokenStore
{
    private const string TokenKey = "auth_token";
    
    public async Task<string?> GetTokenAsync()
    {
        return await localStorage.GetItemAsync<string>(TokenKey);
    }

    public async Task SetTokenAsync(string token)
    {
        await localStorage.SetItemAsync(TokenKey, token);
    }

    public async Task RemoveTokenAsync()
    {
        await localStorage.RemoveItemAsync(TokenKey);
    }
}
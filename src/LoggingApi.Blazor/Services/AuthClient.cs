using LoggingApi.Contracts;
using LoggingApi.Contracts.Logs;

namespace LoggingApi.Blazor.Services.Authentication;

/// <summary>
/// Provides methods for interacting with the authentication endpoints of LoggingApi.
/// </summary>
public sealed class AuthClient(
    HttpClient httpClient) 
{
    public async Task<HttpResponseMessage> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync(
            $"api/v1/Auth/login",
            request,
            cancellationToken);
    }
    
    public async Task<HttpResponseMessage> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        return await httpClient.PostAsJsonAsync(
            $"api/v1/Auth/register",
            request,
            cancellationToken);
    }
}
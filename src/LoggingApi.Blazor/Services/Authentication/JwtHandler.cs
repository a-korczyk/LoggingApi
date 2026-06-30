using System.Net.Http.Headers;

namespace LoggingApi.Blazor.Services.Authentication;

/// <summary>
/// Appends the authorization header to requests.
/// </summary>
public sealed class JwtHandler(
    ITokenStore tokenStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        
        return await base.SendAsync(
            request,
            cancellationToken);
    }
}
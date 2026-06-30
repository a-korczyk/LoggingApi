using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace LoggingApi.Blazor.Services.Authentication;

/// <inheritdoc/>
public sealed class JwtAuthenticationStateProvider(
    ITokenStore tokenStore) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await tokenStore.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal());
        
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwt = jwtSecurityTokenHandler.ReadJwtToken(token);

        var identity = new ClaimsIdentity(
            jwt.Claims,
            authenticationType: "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    /// <param name="token">The JWT token.</param>
    public void NotifyUserAuthenticated(string token)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var jwt = jwtSecurityTokenHandler.ReadJwtToken(token);

        var identity = new ClaimsPrincipal(
            new ClaimsIdentity(
                jwt.Claims,
                authenticationType: "jwt"));

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(identity)));
    }

    public void NotifyUserLoggedOut()
    {
        var anonymous = 
            new ClaimsPrincipal(new ClaimsIdentity());
        
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(anonymous)));
    }
}
using System.Security.Claims;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace LoggingApi.Infrastructure.Services;

/// <summary>
/// Retrieves information about the currently authenticated user
/// from the current HTTP request.
/// </summary>
public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid GetUserId()
    {
        string? userId = httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!Guid.TryParse(userId, out Guid parsedUserId))
            throw new UnauthorizedAccessException();

        return parsedUserId;
    }
}
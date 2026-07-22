using Pingr.Application.Abstractions.Repositories;
using Pingr.Application.Abstractions.Services;
using Pingr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pingr.Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class RefreshTokenRepository(
    ApplicationDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetAsync(
        Guid userId,
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return await dbContext.RefreshTokens.FirstOrDefaultAsync(
            x => x.UserId == userId && x.TokenHash == tokenHash,
            cancellationToken);
    }

    public async Task<ICollection<RefreshToken>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.RefreshTokens
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<RefreshToken>> GetValidByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.RefreshTokens
            .Where(x => 
                x.UserId == userId
                && x.ExpiresAt > DateTimeOffset.UtcNow
                && x.RevokedAt == null)
            .ToListAsync(cancellationToken);
    }
}
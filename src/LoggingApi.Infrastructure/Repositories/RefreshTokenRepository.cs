using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingApi.Infrastructure.Repositories;

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
}
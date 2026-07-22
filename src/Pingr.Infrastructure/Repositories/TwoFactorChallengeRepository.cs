using Pingr.Application.Abstractions.Repositories;
using Pingr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pingr.Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class TwoFactorChallengeRepository(
    ApplicationDbContext dbContext) : ITwoFactorChallengeRepository
{
    public async Task AddAsync(
        TwoFactorChallenge twoFactorChallenge,
        CancellationToken cancellationToken)
    {
        await dbContext.TwoFactorChallenges.AddAsync(
            twoFactorChallenge,
            cancellationToken);
    }
    
    public async Task<TwoFactorChallenge?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.TwoFactorChallenges.FindAsync(
            userId,
            cancellationToken);
    }

    public async Task<TwoFactorChallenge?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return await dbContext.TwoFactorChallenges.FirstOrDefaultAsync(
            x => x.TokenHash == tokenHash,
            cancellationToken);
    }

    public void Delete(
        TwoFactorChallenge twoFactorChallenge)
    {
        dbContext.TwoFactorChallenges.Remove(twoFactorChallenge);
    }
}
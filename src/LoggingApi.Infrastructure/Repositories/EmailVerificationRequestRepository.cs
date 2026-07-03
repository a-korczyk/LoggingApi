using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Entities;

namespace LoggingApi.Infrastructure.Repositories;

/// <inheritdoc/>
public sealed class EmailVerificationRequestRepository(
    ApplicationDbContext dbContext) : IEmailVerificationRequestRepository
{
    public async Task AddAsync(
        EmailVerificationRequest emailVerificationRequest,
        CancellationToken cancellationToken)
    {
        await dbContext.EmailVerificationRequests.AddAsync(
            emailVerificationRequest,
            cancellationToken);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<EmailVerificationRequest?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.EmailVerificationRequests.FindAsync(
            userId, cancellationToken);
    }

    public async Task DeleteAsync(
        EmailVerificationRequest emailVerificationRequest,
        CancellationToken cancellationToken)
    {
        dbContext.EmailVerificationRequests.Remove(emailVerificationRequest);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
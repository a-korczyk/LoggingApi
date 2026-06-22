using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingApi.Infrastructure.Repositories;

/// <summary>
/// Implementation of <see cref="IUserRepository"/>
/// </summary>
public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FindAsync(id, cancellationToken);
    }
    
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(
            x => x.Email == email,
            cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
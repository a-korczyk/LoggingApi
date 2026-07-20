using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using MediatR;

namespace LoggingApi.Application.Features.Users.Commands;

/// <summary>
/// Revokes all valid refresh tokens belonging to a user.
/// </summary>
public sealed record LogoutAllSessionsCommand : IRequest<Result>;

public sealed class LogoutAllSessionsCommandHandler(
    ICurrentUser currentUser,
    IRefreshTokenService refreshTokenService) : IRequestHandler<LogoutAllSessionsCommand, Result>
{
    public async Task<Result> Handle(LogoutAllSessionsCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenService.RevokeValidByUserIdAsync(
            currentUser.GetUserId(),
            cancellationToken);

        return Result.Success();
    }
}
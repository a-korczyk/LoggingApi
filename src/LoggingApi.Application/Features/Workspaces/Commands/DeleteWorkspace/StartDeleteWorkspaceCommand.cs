using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Workspaces.Commands.DeleteWorkspace;

/// <summary>
/// Starts the workspace deletion process.
/// </summary>
/// <param name="WorkspaceId"></param>
public sealed record StartDeleteWorkspaceCommand(
    Guid WorkspaceId) : IRequest<Result<StartDeleteWorkspaceResult>>;
    
public sealed class StartDeleteWorkspaceCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ITwoFactorChallengeRepository twoFactorChallengeRepository,
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<StartDeleteWorkspaceCommand, Result<StartDeleteWorkspaceResult>>
{
    public async Task<Result<StartDeleteWorkspaceResult>> Handle(StartDeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByWorkspaceIdAsync(
            request.WorkspaceId,
            cancellationToken);

        // Check if workspace exists
        if (workspace is null)
            return WorkspaceErrors.NotFound;
        
        var userId = currentUser.GetUserId();
        
        // Check if workspace belongs to user
        if (workspace.OwnerUserId != userId)
            return WorkspaceErrors.ActionForbidden;

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        // Check if user has 2FA enabled
        if (user.TwoFactorEnabled is false)
            return UserErrors.TwoFactorRequired;
        
        var existingChallenge = await twoFactorChallengeRepository.GetAsync(userId, cancellationToken);
        var twoFactorToken = tokenGenerator.GenerateToken();
        
        if (existingChallenge is not null)
        {
            existingChallenge.Update(
                tokenGenerator.HashToken(twoFactorToken),
                TwoFactorChallengePurpose.DeleteWorkspace);
        }
        else
        {
            await twoFactorChallengeRepository.AddAsync(
                new(
                    userId,
                    tokenGenerator.HashToken(twoFactorToken),
                    TwoFactorChallengePurpose.DeleteWorkspace),
                cancellationToken);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StartDeleteWorkspaceResult(
            twoFactorToken);
    }
}

public sealed record StartDeleteWorkspaceResult(
    string twoFactorToken);

public sealed class StartDeleteWorkspaceCommandValidator : AbstractValidator<StartDeleteWorkspaceCommand>
{
    public StartDeleteWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId must not be empty.");
    }
}
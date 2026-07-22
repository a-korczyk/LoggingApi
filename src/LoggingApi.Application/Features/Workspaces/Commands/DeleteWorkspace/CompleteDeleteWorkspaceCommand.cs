using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Application.Features.Users.Commands.DeleteUser;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Workspaces.Commands.DeleteWorkspace;

/// <summary>
/// Completes the workspace deletion process.
/// </summary>
public sealed record CompleteDeleteWorkspaceCommand(
    Guid WorkspaceId,
    string TwoFactorToken,
    string TotpCode) : IRequest<Result>;
    
public sealed class CompleteDeleteWorkspaceCommandHandler(
    IWorkspaceRepository workspaceRepository,
    ITwoFactorService twoFactorService,
    ITwoFactorChallengeRepository twoFactorChallengeRepository,
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<CompleteDeleteWorkspaceCommand, Result>
{
    public async Task<Result> Handle(CompleteDeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetByWorkspaceIdAsync(
            request.WorkspaceId,
            cancellationToken);
        
        // Check if workspace exists
        if (workspace is null)
            return WorkspaceErrors.NotFound;
        
        // Check if workspace belongs to authenticated user
        if (workspace.OwnerUserId != currentUser.GetUserId())
            return WorkspaceErrors.ActionForbidden;
        
        var challenge = await twoFactorChallengeRepository.GetByTokenHashAsync(
            tokenGenerator.HashToken(request.TwoFactorToken),
            cancellationToken);
        
        // Check if challenge exists and if it has the provided purpose
        var challengeValidation = TwoFactorChallenge.ValidateChallenge(
            challenge,
            TwoFactorChallengePurpose.DeleteWorkspace);

        if (challengeValidation.IsFailure)
            return challengeValidation.Error;

        var user = await userRepository.GetByIdAsync(
            challenge.UserId,
            cancellationToken);

        // Check totp code
        var isTotpCodeValid = twoFactorService.VerifyTotpCode(
            request.TotpCode,
            user.TwoFactorSecret);

        if (!isTotpCodeValid)
            return TwoFactorErrors.InvalidTotpCode;

        twoFactorChallengeRepository.Delete(challenge);
        workspaceRepository.Delete(workspace);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}

public sealed class CompleteDeleteWorkspaceCommandValidator : AbstractValidator<CompleteDeleteWorkspaceCommand>
{
    public CompleteDeleteWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("WorkspaceId must not be empty.");
        
        RuleFor(x => x.TwoFactorToken)
            .NotEmpty().WithMessage("TwoFactorToken must not be empty.")
            .MaximumLength(255).WithMessage("TwoFactorToken must not exceed 255 characters.");

        RuleFor(x => x.TotpCode)
            .NotEmpty().WithMessage("TotpCode must not be empty.")
            .Length(6).WithMessage("TotpCode must be 6 digits.")
            .Matches(@"^\d{6}$").WithMessage("TotpCode must contain only digits.");
    }
}

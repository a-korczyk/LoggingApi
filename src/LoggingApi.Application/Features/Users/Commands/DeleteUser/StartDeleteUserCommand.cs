using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Starts the process of deleting a user's account.
/// </summary>
public sealed record StartDeleteUserCommand(
    Guid UserId) : IRequest<Result<StartDeleteUserResponse>>;

public sealed class StartDeleteUserCommandHandler(
    ITwoFactorChallengeRepository twoFactorChallengeRepository,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork, 
    IUserRepository userRepository,
    ICurrentUser currentUser) : IRequestHandler<StartDeleteUserCommand, Result<StartDeleteUserResponse>>
{
    public async Task<Result<StartDeleteUserResponse>> Handle(StartDeleteUserCommand request, CancellationToken cancellationToken)
    {
        // Prevent deleting other users
        if (request.UserId != currentUser.GetUserId())
            return UserErrors.NotFound;
        
        var user = await userRepository.GetByIdAsync(
            currentUser.GetUserId(),
            cancellationToken);

        // Check if user has 2FA enabled
        if (user.TwoFactorEnabled is false)
            return UserErrors.TwoFactorRequired;
        
        var existingChallenge = await twoFactorChallengeRepository.GetAsync(user.Id, cancellationToken);
        var twoFactorToken = tokenGenerator.GenerateToken();
        
        if (existingChallenge is not null)
        {
            existingChallenge.Update(
                tokenGenerator.HashToken(twoFactorToken),
                TwoFactorChallengePurpose.DeleteAccount);
        }
        else
        {
            await twoFactorChallengeRepository.AddAsync(
                new(
                    user.Id,
                    user,
                    tokenGenerator.HashToken(twoFactorToken),
                    TwoFactorChallengePurpose.DeleteAccount),
                cancellationToken);
        }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new StartDeleteUserResponse(
            true,
            twoFactorToken);
    }
}

public sealed record StartDeleteUserResponse(
    bool TwoFactorRequired,
    string? TwoFactorToken);

public sealed class StartDeleteUserCommandValidator : AbstractValidator<StartDeleteUserCommand>
{
    public StartDeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must not be empty.");
    }
}
using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Users.Commands.DeleteUser;

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
        if (request.UserId != currentUser.GetUserId())
            return UserErrors.NotFound;
        
        var user = await userRepository.GetByIdAsync(
            currentUser.GetUserId(),
            cancellationToken);
        
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

        return new StartDeleteUserResponse(twoFactorToken);
    }
}

public sealed record StartDeleteUserResponse(
    string TwoFactorToken);

public sealed class StartDeleteUserCommandValidator : AbstractValidator<StartDeleteUserCommand>
{
    public StartDeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId must not be empty.");
    }
}
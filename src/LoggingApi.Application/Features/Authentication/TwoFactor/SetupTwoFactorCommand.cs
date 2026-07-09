using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Authentication.TwoFactor;

/// <summary>
/// Starts setting up 2FA for a user.
/// </summary>
public sealed record SetupTwoFactorCommand() : IRequest<Result<SetupTwoFactorResponse>>;

public sealed class SetupTwoFactorCommandHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    ITwoFactorChallengeRepository twoFactorChallengeRepository,
    ITwoFactorService twoFactorService) : IRequestHandler<SetupTwoFactorCommand, Result<SetupTwoFactorResponse>>
{
    public async Task<Result<SetupTwoFactorResponse>> Handle(SetupTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(
            currentUser.GetUserId(),
            cancellationToken);

        if (user is null)
            return UserErrors.UserNotFound;
        
        if (user.TwoFactorEnabled)
            return UserErrors.TwoFactorAlreadySetup;

        byte[] secret = twoFactorService.GenerateSecret();
        var twoFactorToken = tokenGenerator.GenerateToken();
        var qrCode = twoFactorService.GenerateQrCode(user.Email, secret);
        
        user.AddTwoFactorSecret(twoFactorService.EncodeSecret(secret));
        await twoFactorChallengeRepository.AddAsync(
            new(
                user.Id,
                user,
                tokenGenerator.HashToken(twoFactorToken),
                TwoFactorChallengePurpose.Confirm2FaSetup),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new SetupTwoFactorResponse(
            twoFactorToken,
            qrCode);
    }
}

/// <summary>
/// Represents the response returned when starting a 2FA setup.
/// </summary>
public sealed record SetupTwoFactorResponse(
    string TwoFactorToken,
    byte[] QrCode);
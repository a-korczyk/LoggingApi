using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Authentication.Login;

/// <summary>
/// Authenticates a user by using their email address and password and either
/// issues a JWT or starts the 2FA flow.
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<LoginResponse>>;

/// <summary>
/// Handles user authentication and decides if 2FA is required.
/// </summary>
public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    ITwoFactorChallengeRepository twoFactorChallengeRepository,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return UserErrors.InvalidCredentials;
        if (user.EmailConfirmed is false)
            return UserErrors.UnverifiedEmail;
        
        bool isPasswordValid = passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash,
            cancellationToken);
        if (!isPasswordValid)
            return UserErrors.InvalidCredentials;

        if (!user.TwoFactorEnabled)
        {
            string jwtToken = jwtProvider.CreateToken(user);
            return new LoginResponse(
                JwtToken: jwtToken,
                TwoFactorToken: null,
                RequiresTwoFactor: false);
        }

        var twoFactorToken = tokenGenerator.GenerateToken();
        await twoFactorChallengeRepository.AddAsync(
            new(
                user.Id,
                user,
                tokenGenerator.HashToken(twoFactorToken),
                TwoFactorChallengePurpose.Login),
            cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            JwtToken: null,
            TwoFactorToken: twoFactorToken,
            RequiresTwoFactor: true);
    }
}

/// <summary>
/// Represents the result of a login attempt, containing either a
/// JWT or a 2FA challenge token.
/// </summary>
public sealed record LoginResponse(
    string? JwtToken,
    string? TwoFactorToken,
    bool RequiresTwoFactor);

/// <summary>
/// Validates data provided when logging in.
/// </summary>
public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        // Email rules
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email must not be empty")
            .EmailAddress().WithMessage("Invalid email address format");
        
        // Password rules
        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password must not be empty");
    }
}
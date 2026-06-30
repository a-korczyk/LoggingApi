using FluentValidation;
using FluentValidation.Validators;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Contracts;
using LoggingApi.Domain;
using LoggingApi.Domain.Entities;
using LoggingApi.Shared;
using MediatR;

namespace LoggingApi.Application.Features.Authentication.Commands;

/// <summary>
/// Authenticates a user by using their email address and password.
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<LoginResponse>>;

/// <summary>
/// Handles user authentication and returns a JWT token if credentials are valid.
/// </summary>
public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
            return UserErrors.InvalidCredentials;
        
        bool isPasswordValid = passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash,
            cancellationToken);
        if (!isPasswordValid)
            return UserErrors.InvalidCredentials;

        string jwtToken = jwtProvider.CreateToken(user);

        return new LoginResponse(jwtToken);
    }
}

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
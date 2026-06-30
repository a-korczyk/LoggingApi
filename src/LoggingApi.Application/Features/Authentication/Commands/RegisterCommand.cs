using FluentValidation;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Contracts;
using LoggingApi.Domain;
using LoggingApi.Domain.Entities;
using LoggingApi.Shared;
using MediatR;

namespace LoggingApi.Application.Features.Authentication.Commands;

/// <summary>
/// Creates a new user and authenticates with the provided <c>Email</c> and <c>Password</c>.
/// </summary>
public sealed record RegisterCommand(
    string Email,
    string Password) : IRequest<Result<RegisterResponse>>;

/// <summary>
/// Handles new user creation and returns a JWT token when the user is created.
/// </summary>
public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.GetByEmailAsync(request.Email, cancellationToken) != null)
            return UserErrors.EmailAlreadyExists;
        
        string hashedPassword = passwordHasher.HashPassword(request.Password, cancellationToken);
        
        User user = new User(
            request.Email,
            hashedPassword);
        
        await userRepository.AddAsync(user, cancellationToken);
        
        string jwtToken = jwtProvider.CreateToken(user);
        
        return new RegisterResponse(jwtToken);
    }
}


/// <summary>
/// Validates data when registering a new user.
/// </summary>
public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        // Email rules
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email must not be empty")
            .MaximumLength(255).WithMessage("Email length must not exceed 255 characters")
            .EmailAddress().WithMessage("Invalid email address format");
        
        // Password rules
        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password must not be empty")
            .MinimumLength(8).WithMessage("Password length must not be less than 8 characters")
            .MaximumLength(255).WithMessage("Password length must not exceed 255 characters");
    }
}

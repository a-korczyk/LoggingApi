using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
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
/// Response returned after a successful registration.
/// </summary>
public sealed record RegisterResponse(
    string JwtToken);
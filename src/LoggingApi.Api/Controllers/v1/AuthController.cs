using LoggingApi.Application.Features.Authentication.Commands;
using LoggingApi.Application.Features.Authentication.Login;
using LoggingApi.Application.Features.Authentication.TwoFactor;
using LoggingApi.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LoggingApi.Api.Controllers.v1;

/// <summary>
/// Provides endpoints related to user authentication and registration.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Authenticates given login details.
    /// </summary>
    /// <param name="request">The login details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="LoginResponse"/> or an <see cref="Error"/>.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.InvalidCredentials" or "Users.UnverifiedEmail" => Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok(response.Value);
    }
    
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Empty 201 status code or an <see cref="Error"/>.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.EmailAlreadyExists" => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Created();
    }

    /// <summary>
    /// Verifies a user's email.
    /// </summary>
    /// <param name="request">The verification details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>204 status code or an <see cref="Error"/>.</returns>
    [HttpGet("verify")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Verify(
        [FromQuery] VerifyCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "EmailVerificationRequests.NotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return NoContent();
    }

    /// <summary>
    /// Completes the login flow if the account has 2FA enabled.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login/2fa")]
    [AllowAnonymous]
    [ProducesResponseType<CompleteTwoFactorLoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTwoFactorLogin(
        [FromBody] CompleteTwoFactorLoginCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.NotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                "TwoFactor.NoChallengeFound" 
                or "TwoFactor.ExpiredChallenge" 
                or "TwoFactor.InvalidToken" 
                or "TwoFactor.InvalidTotpCode" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok(response.Value);
    }

    /// <summary>
    /// Starts the 2FA setup process.
    /// </summary>
    [HttpPost("2fa/setup")]
    [Authorize]
    [ProducesResponseType<SetupTwoFactorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SetupTwoFactor(
        [FromBody] SetupTwoFactorCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.NotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                "Users.TwoFactorAlreadySetup" => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok(response.Value);
    }

    /// <summary>
    /// Completes the 2FA setup process.
    /// </summary>
    [HttpPost("2fa/confirm")]
    [Authorize]
    [ProducesResponseType<ConfirmTwoFactorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmTwoFactor(
        [FromBody] ConfirmTwoFactorCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.NotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                "Users.TwoFactorAlreadySetup" or "Users.TwoFactorSetupNotRequested" => Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: response.Error.Code,
                    detail: response.Error.Message),
                "TwoFactor.NoChallengeFound" 
                or "TwoFactor.ExpiredChallenge" 
                or "TwoFactor.InvalidToken" 
                or "TwoFactor.InvalidTotpCode" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok(response.Value);
    }

    /// <summary>
    /// Gets a user a new access token and rotates their refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType<ConfirmTwoFactorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshAccessTokenCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "RefreshToken.NotFound"
                or "RefreshToken.Expired"
                or "RefreshToken.Revoked"
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response.Value);
    }
}

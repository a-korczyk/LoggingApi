using LoggingApi.Application.Features.Authentication.Commands;
using LoggingApi.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.InvalidCredentials" => Unauthorized(response.Error),
                _ => BadRequest(response.Error)
            };
        
        return Ok(response.Value);
    }
    
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="RegisterResponse"/> or an <see cref="Error"/>.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.EmailAlreadyExists" => Conflict(response.Error),
                _ => BadRequest(response.Error)
            };

        return Created(string.Empty, response.Value);
    }
}

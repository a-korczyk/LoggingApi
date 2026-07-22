using LoggingApi.Application.Features.Users.Commands.DeleteUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApi.Api.Controllers.v1;

/// <summary>
/// Provides endpoints related to users.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Starts the user deletion process.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType<StartDeleteUserCommand>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StartDeleteUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new StartDeleteUserCommand(id),
            cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.TwoFactorRequired" => Problem(
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
    /// Completes the user deletion process.
    /// </summary>
    [HttpDelete("{id:guid}/confirm-deletion")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteDeleteUser(
        [FromRoute] Guid id,
        [FromBody] CompleteDeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { UserId = id},
            cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "TwoFactor.NoChallengeFound" 
                or "TwoFactor.ExpiredChallenge" 
                or "TwoFactor.InvalidToken" 
                or "TwoFactor.InvalidTotpCode" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return NoContent();
    }
}
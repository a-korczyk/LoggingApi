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
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> StartDeleteUser(
        [FromRoute] Guid userId,
        [FromBody] StartDeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { UserId = userId},
            cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response);
    }
    
    [HttpDelete("{id:guid}/confirm-deletion")]
    [Authorize]
    public async Task<IActionResult> CompleteDeleteUser(
        [FromRoute] Guid userId,
        [FromBody] CompleteDeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { UserId = userId},
            cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response);
    }
}
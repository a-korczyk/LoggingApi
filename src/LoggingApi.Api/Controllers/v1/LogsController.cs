using LoggingApi.Application.Features.Logs.Commands;
using LoggingApi.Application.Features.Logs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApi.Api.Controllers.v1;

/// <summary>
/// Provides endpoints related to logs.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/workspaces/{workspaceId:guid}/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class LogsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Adds a new log.
    /// </summary>
    /// <param name="request">The request details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="CreatedAtActionResult"/> or an error.</returns>
    [HttpPost]
    [ProducesResponseType<AddLogResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLog(
        [FromRoute] Guid workspaceId,
        [FromBody] AddLogCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = workspaceId },
            cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return CreatedAtAction(
            nameof(GetLogById),
            new { id = response.Value!.Id},
            response.Value);
    }
    
    /// <summary>
    /// Returns the log with the given identifier.
    /// </summary>
    /// <remarks>
    /// Internally checks if the authenticated user is a member
    /// of the workspace that the log is in.
    /// </remarks>
    /// <param name="id">The identifier of the log.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="LogResponse"/> or an error.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<LogResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLogById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetLogByIdQuery(id),
            cancellationToken);

        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Logs.LogWithIdNotFound" => Problem(
                    statusCode: StatusCodes.Status404NotFound,
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
    /// Returns a paginated collection of logs belonging to the currently authenticated user.
    /// </summary>
    /// <param name="request">The request details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="GetLogsResponse"/> or an error.</returns>
    [HttpGet]
    [ProducesResponseType<GetLogsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLogs(
        [FromRoute] Guid workspaceId,
        [FromQuery] GetLogsQuery request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = workspaceId },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response.Value);
    }

    /// <summary>
    /// Updates the provided log.
    /// </summary>
    /// <param name="id">The log's identifier.</param>
    /// <param name="request">Update details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="OkResult"/> or an error.</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLog(
        [FromRoute] Guid workspaceId,
        [FromRoute] Guid id,
        [FromBody] UpdateLogCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with
            {
                WorkspaceId = workspaceId,
                Id = id
            }, 
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Logs.LogWithIdNotFound" => Problem(
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
    /// Deletes the provided log.
    /// </summary>
    /// <remarks>
    /// Internally checks if the authenticated user is a member
    /// of the workspace that the log is in.
    /// </remarks>
    /// <param name="id">The log's identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see cref="OkResult"/> or an error.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLog(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new DeleteLogCommand(id),
            cancellationToken);
            
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Logs.LogWithIdNotFound" => Problem(
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
}
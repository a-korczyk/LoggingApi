using LoggingApi.Application.Features.Users.Commands.DeleteUser;
using LoggingApi.Application.Features.Workspaces.Commands;
using LoggingApi.Application.Features.Workspaces.Commands.DeleteWorkspace;
using LoggingApi.Application.Features.Workspaces.Commands.TransferOwnership;
using LoggingApi.Application.Features.Workspaces.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApi.Api.Controllers.v1;

/// <summary>
/// Provides endpoints related to workspaces.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class WorkspacesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new workspace.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType<AddWorkspaceResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWorkspace(
        [FromBody] AddWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return CreatedAtAction(
            nameof(GetWorkspaceById),
            new { id = response.Value.WorkspaceId },
            response.Value);
    }
    
    /// <summary>
    /// Adds a new user to the workspace.
    /// </summary>
    [HttpPost("{id:guid}/users/add")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddWorkspaceUser(
        [FromRoute] Guid id,
        [FromBody] AddWorkspaceUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = id },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok();
    }
    
    /// <summary>
    /// Changes a role of a user in a workspace.
    /// </summary>
    [HttpPost("{id:guid}/users/{userId:guid}/change-role")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeWorkspaceUserRole(
        [FromRoute] Guid id,
        [FromRoute] Guid userId,
        [FromBody] ChangeWorkspaceUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with
            {
                WorkspaceId = id,
                UserId = userId
            },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok();
    }
    
    /// <summary>
    /// Leaves the user from the workspace.
    /// </summary>
    [HttpPost("{id:guid}/leave")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LeaveWorkspace(
        [FromRoute] Guid id,
        [FromBody] LeaveWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = id },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok();
    }
    
    /// <summary>
    /// Removes ("kicks") a user from the workspace.
    /// </summary>
    [HttpPost("{id:guid}/users/remove")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteWorkspaceUser(
        [FromRoute] Guid id,
        [FromBody] DeleteWorkspaceUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = id },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };

        return Ok();
    }

    /// <summary>
    /// Retrieves a workspace by its identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType<WorkspaceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWorkspaceById(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetWorkspaceByIdQuery(id),
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Workspace.NotFound" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response.Value);
    }
    
    /// <summary>
    /// Retrieves all workspaces the user is a member of.
    /// </summary>
    [HttpGet]
    [Authorize]
    [ProducesResponseType<GetAllWorkspacesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllWorkspaces(
        [FromQuery] GetAllWorkspacesQuery request, 
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request,
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
    /// Gets all the workspaces a user is the owner of.
    /// </summary>
    [HttpGet("owned")]
    [Authorize]
    [ProducesResponseType<GetOwnedWorkspacesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOwnedWorkspaces(
        [FromQuery] GetOwnedWorkspacesQuery request, 
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request,
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
    /// Starts the workspace ownership transfer process.
    /// </summary>
    [HttpPost("{id:guid}/transfer-ownership")]
    [Authorize]
    [ProducesResponseType<StartTransferOwnershipResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartTransferOwnership(
        [FromRoute] Guid id, 
        [FromBody] StartTransferOwnershipCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = id },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Workspace.ActionForbidden"
                or "Users.TwoFactorRequired"
                or "Users.NotFound"
                or "Workspace.NotFound" 
                or "Workspace.UserNotFound" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response.Value);
    }
    
    /// <summary>
    /// Completes the workspace ownership transfer process.
    /// </summary>
    [HttpPost("{id:guid}/transfer-ownership/confirm")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteTransferOwnership(
        [FromRoute] Guid id, 
        [FromBody] CompleteTransferOwnershipCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = id },
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Users.TwoFactorRequired"
                or "Users.NotFound"
                
                or "Workspace.ActionForbidden"
                or "Workspace.NotFound" 
                or "Workspace.UserNotFound" 
                
                or "TwoFactor.NoChallengeFound" 
                or "TwoFactor.ExpiredChallenge" 
                or "TwoFactor.InvalidToken" 
                or "TwoFactor.InvalidTotpCode" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok();
    }
    
    /// <summary>
    /// Starts the workspace deletion process.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType<StartDeleteWorkspaceResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartDeleteWorkspace(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new StartDeleteWorkspaceCommand(id),
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Workspace.ActionForbidden"
                or"Users.TwoFactorRequired"
                or "Workspace.NotFound" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok(response.Value);
    }
    
    /// <summary>
    /// Completes the workspace deletion process.
    /// </summary>
    [HttpPost("{id:guid}/confirm-deletion")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteDeleteWorkspace(
        [FromRoute] Guid id, 
        [FromBody] CompleteDeleteWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            request with { WorkspaceId = id},
            cancellationToken);
        
        if (response.IsFailure)
            return response.Error.Code switch
            {
                "Workspace.ActionForbidden"
                or "Workspace.NotFound" 
                or "TwoFactor.NoChallengeFound" 
                or "TwoFactor.ExpiredChallenge" 
                or "TwoFactor.InvalidToken" 
                or "TwoFactor.InvalidTotpCode" 
                or _ => Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: response.Error.Code,
                    detail: response.Error.Message)
            };
        
        return Ok();
    }
}
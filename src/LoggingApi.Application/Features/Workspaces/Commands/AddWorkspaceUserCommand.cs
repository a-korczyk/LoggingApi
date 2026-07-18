using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Workspaces.Commands;

/// <summary>
/// Adds a new user to a workspace.
/// </summary>
/// <param name="UserId">User's identifier.</param>
/// <param name="WorkspaceId">Workspace's identifier.</param>
public sealed record AddWorkspaceUserCommand(
    Guid UserId,
    Guid WorkspaceId) : IRequest<Result>;

public sealed class AddWorkspaceUserCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceUserRepository workspaceUserRepository,
    IUserRepository userRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<AddWorkspaceUserCommand, Result>
{
    public async Task<Result> Handle(AddWorkspaceUserCommand request, CancellationToken cancellationToken)
    {
        // Check if workspace exists
        if (await workspaceRepository.GetByWorkspaceIdAsync(request.WorkspaceId, cancellationToken) is null)
            return WorkspaceErrors.NotFound;
        
        // Check if requesting user has authorization to add new workspace users 
        var requestingUserId = currentUser.GetUserId();

        var isRequestingUserAuthorized = await workspaceUserRepository.IsInRoleAsync(
            requestingUserId,
            request.WorkspaceId,
            [WorkspaceRole.Admin, WorkspaceRole.Owner],
            cancellationToken);
        
        if (isRequestingUserAuthorized is false)
            return WorkspaceErrors.ActionForbidden;
        
        // Does target user exist
        if (await userRepository.GetByIdAsync(request.UserId, cancellationToken) is null)
            return UserErrors.NotFound;
        
        // Does target user already exist in the workspace
        var isTargetUserInWorkspace = await workspaceUserRepository.IsMemberAsync(
            request.UserId,
            request.WorkspaceId,
            cancellationToken);

        if (isTargetUserInWorkspace)
            return WorkspaceErrors.UserAlreadyInWorkspace;

        // Add new workspace user
        var newWorkspaceUser = new WorkspaceUser(
            request.WorkspaceId,
            request.UserId,
            WorkspaceRole.User);
        
        await workspaceUserRepository.AddAsync(newWorkspaceUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
    
public sealed class AddWorkspaceUserCommandValidator : AbstractValidator<AddWorkspaceUserCommand>
{
    public AddWorkspaceUserCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty().WithMessage("Workspace identifier must not be empty.");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User identifier must not be empty.");
    }
}

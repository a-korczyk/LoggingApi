using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Workspaces.Commands;

/// <summary>
/// Creates a new workspace and adds the user as the owner.
/// </summary>
/// <param name="Name">Name of the workspace.</param>
public sealed record AddWorkspaceCommand(
    string Name) : IRequest<Result<AddWorkspaceResponse>>;

public sealed class AddWorkspaceCommandHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceUserRepository workspaceUserRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<AddWorkspaceCommand, Result<AddWorkspaceResponse>>
{
    public async Task<Result<AddWorkspaceResponse>> Handle(AddWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        
        // Add workspace
        var workspace = new Workspace(
            userId,
            request.Name);
        
        await workspaceRepository.AddAsync(
            workspace,
            cancellationToken);

        // Add workspace user
        var workspaceUser = new WorkspaceUser(
            workspace.Id,
            userId,
            WorkspaceRole.Owner);
        
        await workspaceUserRepository.AddAsync(
            workspaceUser,
            cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddWorkspaceResponse(workspace.Id);
    }
}

/// <summary>
/// Response representation of a successful workspace creation.
/// </summary>
/// <param name="WorkspaceId"></param>
public sealed record AddWorkspaceResponse(
    Guid WorkspaceId);
    
public sealed class AddWorkspaceCommandValidator : AbstractValidator<AddWorkspaceCommand>
{
    public AddWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name must not be empty.")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters");
    }
}

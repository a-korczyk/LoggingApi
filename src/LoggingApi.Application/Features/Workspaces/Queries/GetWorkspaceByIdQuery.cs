using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using MediatR;

namespace LoggingApi.Application.Features.Workspaces.Queries;

/// <summary>
/// Gets a workspace where a user is a member.
/// </summary>
public sealed record GetWorkspaceByIdQuery(
    Guid WorkspaceId) : IRequest<Result<WorkspaceResponse>>;
    
public sealed class GetWorkspaceByIdQueryHandler(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceUserRepository workspaceUserRepository,
    IUserRepository userRepository,
    ICurrentUser currentUser) : IRequestHandler<GetWorkspaceByIdQuery, Result<WorkspaceResponse>>
{
    public async Task<Result<WorkspaceResponse>> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var isMember = await workspaceUserRepository.IsMemberAsync(currentUser.GetUserId(), request.WorkspaceId, cancellationToken);
        
        if (isMember is false)
            return WorkspaceErrors.NotFound;
        
        var workspace = await workspaceRepository.GetByWorkspaceIdAsync(
            request.WorkspaceId,
            cancellationToken);
        
        if (workspace is null)
            return WorkspaceErrors.NotFound;

        return new WorkspaceResponse(
            workspace.Id,
            workspace.OwnerUserId,
            workspace.Name,
            workspace.CreatedAt);
    }
}

public sealed record WorkspaceResponse(
    Guid WorkspaceId,
    Guid OwnerUserId,
    string Name,
    DateTimeOffset CreatedAt);
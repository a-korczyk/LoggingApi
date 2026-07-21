using FluentValidation;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Application.Features.Logs.Queries;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Workspaces.Queries;

/// <summary>
/// Gets all the workspaces a user is a member in.
/// </summary>
public sealed record GetAllWorkspacesQuery(
    int? Page,
    int? PageSize) : IRequest<Result<GetAllWorkspacesResponse>>;

public sealed class GetAllWorkspacesQueryHandler(
    IWorkspaceUserRepository workspaceUserRepository,
    IWorkspaceRepository workspaceRepository,
    ICurrentUser currentUser) : IRequestHandler<GetAllWorkspacesQuery, Result<GetAllWorkspacesResponse>>
{
    public async Task<Result<GetAllWorkspacesResponse>> Handle(GetAllWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var allWorkspaces = await workspaceRepository.GetByUserIdAsync(
            currentUser.GetUserId(),
            new Pagination(
                request.Page ?? Pagination.DefaultPage,
                request.PageSize ?? Pagination.DefaultPageSize),
            cancellationToken);

        return new GetAllWorkspacesResponse(
            allWorkspaces
                .Select(x => new WorkspaceResponse(
                    x.Id,
                    x.OwnerUserId,
                    x.Name,
                    x.CreatedAt))
                .ToList());
    }
}

public sealed record GetAllWorkspacesResponse(
    ICollection<WorkspaceResponse> AllWorkspaces);

public sealed class GetAllWorkspacesQueryValidator : AbstractValidator<GetAllWorkspacesQuery>
{
    public GetAllWorkspacesQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero")
            .When(query => query.Page != null);

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than zero")
            .LessThan(101).WithMessage("PageSize must not be greater than 100")
            .When(query => query.PageSize != null);
    }
}
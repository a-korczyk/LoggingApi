using System.Collections.Immutable;
using FluentValidation;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Logs.Queries;

/// <summary>
/// Retrieves a paginated collection of logs belonging to the currently authenticated user.
/// </summary>
public sealed record GetLogsQuery(
    int Page,
    int PageSize) : IRequest<Result<GetLogsResponse>>;
    
/// <summary>
/// Handles <see cref="GetLogsQuery"/> requests.
/// </summary>
public sealed class GetLogsQueryHandler(
    ILogRepository logRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetLogsQuery, Result<GetLogsResponse>>
{
    public async Task<Result<GetLogsResponse>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Log> logs = await logRepository.GetAsync(
            currentUser.GetUserId(),
            new Pagination(
                request.Page,
                request.PageSize),
            cancellationToken);

        IReadOnlyList<LogResponse> responseLogs = logs
            .Select(log => new LogResponse(
                log.Id, 
                log.Status,
                log.Type,
                log.Title,
                log.Data,
                log.CreatedAt))
            .ToList();

        return new GetLogsResponse(responseLogs);
    }
}

/// <summary>
/// Response on successful retrieval of all logs associated with a user.
/// </summary>
public sealed record GetLogsResponse(
    IReadOnlyList<LogResponse> Logs);

/// <summary>
/// Validates <see cref="GetLogsQuery"/> requests.
/// </summary>
public sealed class GetLogsValidator : AbstractValidator<GetLogsQuery>
{
    public GetLogsValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0).WithMessage("Page must be greater than zero");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than zero")
            .LessThan(101).WithMessage("PageSize must not be greater than 100");
    }
}
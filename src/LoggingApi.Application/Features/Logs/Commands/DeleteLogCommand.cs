using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Logs.Commands;

/// <summary>
/// Deletes the log with the provided identifier.
/// </summary>
public sealed record DeleteLogCommand(
    Guid Id) : IRequest<Result>;

/// <summary>
/// Handles <see cref="DeleteLogCommand"/> requests.
/// </summary>
public sealed class DeleteLogCommandHandler(
    ILogRepository logRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    ILogDigestQueue digestQueue)
    : IRequestHandler<DeleteLogCommand, Result>
{
    public async Task<Result> Handle(DeleteLogCommand request, CancellationToken cancellationToken)
    {
        Log? log = await logRepository.GetByIdAsync(request.Id, cancellationToken);

        // TODO: check if user has workspaceuser with workspaceid matching log's
        if (log == null /* || currentuser.getuserid() != log.userid */)
            return LogErrors.LogWithIdNotFound;

        logRepository.Delete(log);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try {
            await digestQueue.DeleteAsync(
                currentUser.GetUserEmail(),
                log.Id);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.ToString());
        }
        
        return Result.Success();
    }
}

/// <summary>
/// Validates <see cref="DeleteLogCommand"/> requests.
/// </summary>
public sealed class DeleteLogValidator : AbstractValidator<DeleteLogCommand>
{
    public DeleteLogValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id must not be empty");
    }
}
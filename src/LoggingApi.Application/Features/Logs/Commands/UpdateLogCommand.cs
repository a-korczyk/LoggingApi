using System.Text.Json;
using FluentValidation;
using LoggingApi.Application.Abstractions;
using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using LoggingApi.Application.Abstractions.Services.Email;
using LoggingApi.Domain.Common;
using LoggingApi.Domain.Entities;
using MediatR;

namespace LoggingApi.Application.Features.Logs.Commands;

/// <summary>
/// Updates an existing log with provided details.
/// </summary>
/// <param name="Id">Identifier of the log to update.</param>
/// <param name="Status">The log's new <see cref="LogStatus"/>.</param>
/// <param name="Type">The log's new <see cref="LogType"/>.</param>
/// <param name="Title">The log's new title.</param>
/// <param name="Data">Updated additional log details.</param>
public sealed record UpdateLogCommand(
    Guid Id,
    LogStatus? Status,
    LogType? Type,
    string? Title,
    JsonDocument? Data) : IRequest<Result>;

/// <summary>
/// Handles <see cref="UpdateLogCommand"/> requ.
/// </summary>
public sealed class UpdateLogCommandHandler(
    ILogRepository logRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IUserRepository userRepository,
    ILogNotificationService logNotificationService,
    ILogDigestQueue digestQueue)
    : IRequestHandler<UpdateLogCommand, Result>
{
    public async Task<Result> Handle(UpdateLogCommand request, CancellationToken cancellationToken)
    {
        Log? log = await logRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (log == null || currentUser.GetUserId() != log.UserId)
            return LogErrors.LogWithIdNotFound;
        
        
        log.Update(
            request.Status,
            request.Type,
            request.Title,
            request.Data);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        
        var user = await userRepository.GetByIdAsync(currentUser.GetUserId(), cancellationToken);

        try
        {
            if (log.Type == LogType.CriticalError && log.Status == LogStatus.Resolved)
                await logNotificationService.NotifyCriticalErrorAsync(
                    log,
                    user,
                    cancellationToken);

            digestQueue.Update(
                user.Email,
                new LogDigestEntry(
                    log.Id,
                    log.Status,
                    log.Type,
                    log.Title));
        }
        catch
        {
            await Console.Error.WriteLineAsync(ex.ToString());
        }

        
        return Result.Success();
    }
}

/// <summary>
/// Validates <see cref="UpdateLogCommand"/> requests.
/// </summary>
public sealed class UpdateLogCommandValidator : AbstractValidator<UpdateLogCommand>
{
    public UpdateLogCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Id must not be empty");
        
        RuleFor(command => command.Status)
            .IsInEnum().WithMessage("Status must match LogStatus enum.")
            .When(command => command.Status != null);
        
        RuleFor(command => command.Type)
            .IsInEnum().WithMessage("Type must match LogType enum.")
            .When(command => command.Type != null);

        RuleFor(command => command.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title)).WithMessage("Title must not be empty.")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.")
            .When(command => command.Data != null);
    }
}
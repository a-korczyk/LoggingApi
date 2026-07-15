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
/// Adds a log with the provided details.
/// </summary>
/// <param name="Type">The log's <see cref="LogType"/>.</param>
/// <param name="Title">The log's title.</param>
/// <param name="Data">Additional custom details.</param>
public sealed record AddLogCommand(
    LogType Type,
    string Title,
    JsonDocument Data) : IRequest<Result<AddLogResponse>>;

/// <summary>
/// Handles adding the log and returns <see cref="AddLogResponse"/>.
/// </summary>
public sealed class AddLogCommandHandler(
    ILogRepository logRepository,
    IUserRepository userRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    ILogNotificationService logNotificationService,
    ILogDigestQueue digestQueue)
    : IRequestHandler<AddLogCommand, Result<AddLogResponse>>
{
    public async Task<Result<AddLogResponse>> Handle(AddLogCommand request, CancellationToken cancellationToken)
    {
        Guid userId = currentUser.GetUserId();
        string userEmail = currentUser.GetUserEmail();
        User? user = await userRepository.GetByIdAsync(userId, cancellationToken);
        
        Log log = new Log(
            userId,
            user!,
            request.Type,
            request.Title,
            request.Data);
        
        await logRepository.AddAsync(log, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            if (log.Type == LogType.CriticalError)
                await logNotificationService.NotifyCriticalErrorAsync(
                    log,
                    user,
                    cancellationToken);

            await digestQueue.UpsertAsync(
                userEmail,
                new LogDigestEntry(
                    log.Id,
                    log.Status,
                    log.Type,
                    log.Title)
            );
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync(ex.ToString());
        }

        return new AddLogResponse(
            log.Id);
    }
}

/// <summary>
/// Response after a log has been successfully added.
/// </summary>
/// <param name="Id">The identifier of the newly added log.</param>
public sealed record AddLogResponse(
    Guid Id);

/// <summary>
/// Validates <see cref="AddLogCommand"/> requests.
/// </summary>
public sealed class AddLogValidator : AbstractValidator<AddLogCommand>
{
    public AddLogValidator()
    {
        RuleFor(command => command.Type)
            .IsInEnum().WithMessage("Type must match LogType enum.");

        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("Title must not be empty.")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");
    }
}
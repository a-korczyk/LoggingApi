using System.Text.Json;
using LoggingApi.Domain.Entities;

namespace LoggingApi.Contracts.Logs;

/// <summary>
/// Request sent to update a log.
/// </summary>
/// <param name="Id">Identifier of the log to update.</param>
/// <param name="Status">The log's new <see cref="LogStatus"/>.</param>
/// <param name="Type">The log's new <see cref="LogType"/>.</param>
/// <param name="Title">The log's new title.</param>
/// <param name="Data">Updated additional log details.</param>
public sealed record UpdateLogRequest(
    Guid Id,
    LogStatus? Status,
    LogType? Type,
    string? Title,
    JsonDocument? Data);

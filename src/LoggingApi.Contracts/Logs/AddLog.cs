using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using LoggingApi.Domain.Entities;

namespace LoggingApi.Contracts.Logs;

/// <summary>
/// Request sent to add a new log.
/// </summary>
/// <param name="Type">The log's <see cref="LogType"/>.</param>
/// <param name="Title">The log's title.</param>
/// <param name="Data">Additional custom details.</param>
public sealed record AddLogRequest(
    [Required]
    LogType Type,
    
    [Required]
    string Title,
    
    [Required]
    JsonDocument Data);

/// <summary>
/// Response after a log has been successfully added.
/// </summary>
/// <param name="Id">The identifier of the newly added log.</param>
public sealed record AddLogResponse(
    Guid Id);

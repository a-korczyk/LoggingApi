using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using LoggingApi.Domain.Entities;

namespace LoggingApi.Contracts.Logs;

/// <summary>
/// The response representation of an individual log.
/// </summary>
public sealed record LogResponse(
    Guid Id,
    LogStatus Status,
    LogType Type,
    string Title,
    JsonDocument Data,
    DateTimeOffset CreatedAt);

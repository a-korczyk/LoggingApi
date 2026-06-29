namespace LoggingApi.Contracts.Logs;

/// <summary>
/// Request sent to get a paginated collection of logs belonging to the user.
/// </summary>
public sealed record GetLogsRequest(
    int? Page,
    int? PageSize);

/// <summary>
/// Response on successful retrieval of all logs associated with a user.
/// </summary>
public sealed record GetLogsResponse(
    IReadOnlyList<LogResponse> Logs);

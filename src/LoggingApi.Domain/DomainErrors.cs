using LoggingApi.Shared;

namespace LoggingApi.Domain;

/// <summary>
/// Contains application errors related to users.
/// </summary>
public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new(
            "Users.InvalidCredentials",
            "The provided email or password are invalid.");
    
    public static readonly Error EmailAlreadyExists =
        new(
            "Users.EmailAlreadyExists",
            "A user with the provided email already exists.");
}

/// <summary>
/// Contains application errors related to logs.
/// </summary>
public sealed class LogErrors
{
    public static readonly Error LogWithIdNotFound =
        new(
            "Logs.LogWithIdNotFound",
            "Couldn't find a log with the provided Id.");

    public static readonly Error Forbidden =
        new(
            "Logs.Forbidden",
            "You don't have access this log.");
}

/// <summary>
/// Contains application errors related to validation.
/// </summary>
public static class ValidationErrors
{
    public static readonly Error Failed =
        new(
            "Validation.Failed",
            "Validation failed.");
}

/// <summary>
/// Contains application errors related to the server.
/// </summary>
public static class ServerErrors
{
    public static readonly Error InternalError =
        new(
            "Server.InternalError",
            string.Empty);
}

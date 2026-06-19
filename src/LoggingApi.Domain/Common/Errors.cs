namespace LoggingApi.Domain.Common;

/// <summary>
/// Represents an application error.
/// </summary>
/// <param name="Code">A unique error code.</param>
/// <param name="Message">A human-readable error message.</param>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    /// Represents the absence of an error.
    /// Used by a successful <see cref="Result{T}"/> instance.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);
}

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

namespace LoggingApi.Shared;

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

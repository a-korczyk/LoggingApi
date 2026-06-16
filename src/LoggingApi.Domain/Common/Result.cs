namespace LoggingApi.Domain.Common;

/// <summary>
/// Implementation of the Result pattern.
/// Represents the outcome of an operation that can succeed with a value or fail with an error.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <remarks>
/// A successful result contains a value and <see cref="Error.None"/>.
/// A failed result contains and error and no value.
/// </remarks>
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error Error { get; }

    private Result(bool isSuccess, T? value, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            throw new ArgumentException("Invalid result state", nameof(error));
        
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, Error.None);
    
    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    public static Result<T> Failure(Error error) => new(false, default, error);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}

namespace Pingr.Domain.Common;

/// <summary>
/// Non-generic implementation of the Result pattern.
/// Represents the outcome of an operation that can succeed or fail with an error.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    
    /// <summary>
    /// Gets the error associated with a failed result.
    /// </summary>
    public Error Error { get; }
    
    
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            throw new ArgumentException("Invalid result state", nameof(error));
        
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, Error.None);
    
    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Generic implementation of the Result pattern.
/// Represents the outcome of an operation that can succeed with a value or fail with an error.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <remarks>
/// A successful result contains a value and <see cref="Error.None"/>.
/// A failed result contains an error and no value.
/// </remarks>
public sealed class Result<T> : Result
{
    /// <summary>
    /// Gets the value of a successful result.
    /// </summary>
    public T? Value { get; }

    private Result(bool isSuccess, T? value, Error error) : base(isSuccess, error)
    {
        Value = value;
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

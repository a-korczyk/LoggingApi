using FluentValidation;
using MediatR;

namespace LoggingApi.Application.Behaviors;

/// <summary>
/// Executes FluentValidation validators for incoming requests
/// before passing them along to the next handler in the pipeline.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();
        if (failures.Any())
            throw new ValidationException(failures);
        
        return await next(cancellationToken);
    }
}
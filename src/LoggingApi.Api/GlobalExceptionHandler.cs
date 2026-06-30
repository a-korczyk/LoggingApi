using System.Diagnostics;
using FluentValidation;
using LoggingApi.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApi.Api;

/// <summary>
/// Handles unhandled exceptions and converts them into
/// HTTP <see cref="ProblemDetails"/> responses.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = exception switch
        {
            // ValidationException
            ValidationException validationException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = ValidationErrors.Failed.Code,
                Extensions = 
                {
                    ["errors"] = validationException.Errors
                        .GroupBy(g => g.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage).ToArray())
                }
            },

            // Unhandled exceptions
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = ServerErrors.InternalError.Code,
            }
        };
        
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);
        
        return true;
    }
}
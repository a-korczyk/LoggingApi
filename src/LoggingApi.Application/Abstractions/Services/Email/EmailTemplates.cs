using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Services.Email;

/// <remarks><c>Body</c> must be in Markdown format.</remarks>
public sealed record EmailTemplate(
    string Subject,
    string Body);

/// <summary>
/// Provides predefined email templates for application notifications.
/// </summary>
public static class EmailTemplates
{
    public static EmailTemplate CriticalErrorLogged(
        Log log) =>
        new(
            "Critical Error Logged - LoggingApi",
            $"""
             # Critical Error Logged
             A critical error has been registered and requires immediate attention.
             
             ## Error Details:
             - Identifier: {log.Id}
             - Title: {log.Title}
             - Logged At (UTC): {log.CreatedAt}
             
             This email was generated automatically by LoggingApi.
             """
        );
    
    public static EmailTemplate CriticalErrorResolved(
        Log log) =>
        new(
            "Resolved Critical Error - LoggingApi",
            $"""
             # Critical Error Resolved

             ## Resolved Error Details:
             - Identifier: {log.Id}
             - Title: {log.Title}
             - Logged At (UTC): {log.CreatedAt}

             This email was generated automatically by LoggingApi.
             """
        );

    public static readonly EmailTemplate LogDigest =
        new(
            "Log Summary (Last 30 Minutes) - LoggingApi",
            string.Empty);

    public static readonly EmailTemplate LogDigestFailed = 
        new(
            "Log Summary (Last 30 Minutes) - LoggingApi",
            $"""
             # Log Summary From The Last 30 Minutes
             
             There was a problem with generating the AI summary.
             
             This email was generated automatically by LoggingApi.
             """);
}
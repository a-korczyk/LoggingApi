namespace LoggingApi.Application.Abstractions.Services.Email;

/// <summary>
/// Provides methods for sending emails.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="messageDetails">The email message details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SendAsync(
        EmailMessageDetails messageDetails,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents the details of an email message.
/// </summary>
/// <param name="RecipientEmail">The recipient's email address.</param>
/// <param name="RecipientName">The recipient's name, if available.</param>
/// <param name="Subject">The email subject.</param>
/// <param name="MarkdownBody">The body of the email in Markdown format.</param>
public sealed record EmailMessageDetails(
    string RecipientEmail,
    string? RecipientName,
    string Subject,
    string MarkdownBody);
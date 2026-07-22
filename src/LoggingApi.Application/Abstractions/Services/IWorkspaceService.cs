using LoggingApi.Application.Abstractions.Services.Email;

namespace LoggingApi.Application.Abstractions.Services;

/// <summary>
/// Provides workspace related methods.
/// </summary>
public interface IWorkspaceService
{
    Task SendEmailToEveryMemberAsync(
        Guid workspaceId,
        EmailMessageDetails emailMessageDetails,
        CancellationToken cancellationToken);
}
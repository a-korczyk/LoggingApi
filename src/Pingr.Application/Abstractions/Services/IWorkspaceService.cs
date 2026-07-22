using Pingr.Application.Abstractions.Services.Email;

namespace Pingr.Application.Abstractions.Services;

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
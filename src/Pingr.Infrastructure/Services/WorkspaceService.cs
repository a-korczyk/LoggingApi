using Pingr.Application.Abstractions.Repositories;
using Pingr.Application.Abstractions.Services;
using Pingr.Application.Abstractions.Services.Email;
using Pingr.Domain.Entities;

namespace Pingr.Infrastructure.Services;

/// <inheritdoc/>
public sealed class WorkspaceService(
    IUserRepository userRepository,
    IEmailSender emailSender) : IWorkspaceService
{
    public async Task SendEmailToEveryMemberAsync(
        Guid workspaceId,
        EmailMessageDetails emailMessageDetails,
        CancellationToken cancellationToken)
    {
        var currentPage = 1;
        var pageSize = 100;
        
        while (true)
        {
            var workspaceUsers = await userRepository.GetByWorkspaceId(
                workspaceId,
                new Pagination(
                    currentPage,
                    pageSize),
                cancellationToken);

            if (!workspaceUsers.Any())
                break;

            foreach (var user in workspaceUsers)
            {
                var message = emailMessageDetails with { RecipientEmail =  user.Email };

                try
                {
                    await emailSender.SendAsync(
                        message,
                        cancellationToken);
                }
                catch (Exception exception)
                {
                    Console.Error.WriteLine(exception.Message);
                }
            }
            
            // Break if count is smaller than pagesize since that
            // means there are no more users left
            if (workspaceUsers.Count < pageSize)
                break;

            currentPage++;
        }
    }
}
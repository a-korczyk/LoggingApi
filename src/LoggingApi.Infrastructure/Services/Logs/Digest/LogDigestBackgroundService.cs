using LoggingApi.Application.Abstractions.Repositories;
using LoggingApi.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IEmailSender = LoggingApi.Application.Abstractions.Services.Email.IEmailSender;

namespace LoggingApi.Infrastructure.Services.Logs.Digest;

/// <summary>
/// Periodically processes log digests by generating and sending
/// summary emails to each user of the workspace.
/// </summary>
public sealed class LogDigestBackgroundService(
    ILogDigestQueue logDigestQueue,
    ILogDigestEmailBuilder logDigestEmailBuilder,
    IServiceScopeFactory serviceScopeFactory,
    IEmailSender emailSender) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var workspaces = await logDigestQueue.TakeWorkspacesAsync();

            foreach (var workspace in workspaces)
            {
                var genericMessage = await logDigestEmailBuilder.Build(
                    workspace,
                    stoppingToken);
                
                if (genericMessage is null)
                    continue;

                var currentPage = 1;
                var pageSize = 100;
                while (true)
                {
                    var workspaceUsers = await userRepository.GetByWorkspaceId(
                        workspace.Key,
                        new Pagination(
                            currentPage,
                            pageSize),
                        stoppingToken);

                    if (!workspaceUsers.Any())
                        break;

                    foreach (var user in workspaceUsers)
                    {
                        var message = genericMessage with { RecipientEmail =  user.Email };
                        
                        await emailSender.SendAsync(
                            message,
                            stoppingToken);
                    }
                    
                    if (workspaceUsers.Count < pageSize)
                        break;

                    currentPage++;
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(0.5), stoppingToken);
        }

    }
}
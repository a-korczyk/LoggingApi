using LoggingApi.Application.Abstractions.Services;
using Microsoft.Extensions.Hosting;
using IEmailSender = LoggingApi.Application.Abstractions.Services.Email.IEmailSender;

namespace LoggingApi.Infrastructure.Services.Logs.Digest;

/// <summary>
/// Periodically processes log digests by generating and sending
/// summary emails to each recipient.
/// </summary>
public sealed class LogDigestBackgroundService(
    ILogDigestQueue logDigestQueue,
    ILogDigestEmailBuilder logDigestEmailBuilder,
    IEmailSender emailSender) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var recipients = await logDigestQueue.TakeRecipientsAsync();

            foreach (var recipient in recipients)
            {
                var message = await logDigestEmailBuilder.Build(
                    recipient,
                    stoppingToken);

                await emailSender.SendAsync(
                    message,
                    stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }

    }
}
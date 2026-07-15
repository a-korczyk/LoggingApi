using LoggingApi.Application.Abstractions.Services;

namespace LoggingApi.Infrastructure.Services.Logs.Digest;

/// <inheritdoc/>
public sealed class LogDigestQueue(
    ICacheService cacheService) : ILogDigestQueue
{
    public async Task UpsertAsync(
        string email,
        LogDigestEntry entry)
    {
        await cacheService.HashSetAsync(
            $"log-digest-queue:recipients:{email}",
            entry.Id.ToString(),
            entry
        );
        
        await cacheService.HashSetIfNotExistsAsync(
            "log-digest-queue:recipients-list",
            email,
            true);
    }

    public async Task DeleteAsync(
        string email,
        Guid id)
    {
        await cacheService.HashDeleteAsync(
            $"log-digest-queue:recipients:{email}",
            id.ToString());
        
        var recipientDigest = 
            await cacheService
                .HashGetAllAsync<LogDigestEntry>($"log-digest-queue:recipients:{email}");

        if (!recipientDigest.Any())
            await cacheService.HashDeleteAsync(
                "log-digest-queue:recipients-list",
                email);
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyDictionary<Guid, LogDigestEntry>>> TakeRecipientsAsync()
    {
        var result = new Dictionary<string, IReadOnlyDictionary<Guid, LogDigestEntry>>();
        
        var recipientList = 
            await cacheService
                .HashGetAllAsync<bool>("log-digest-queue:recipients-list");

        foreach (var recipient in recipientList)
        { 
            var cachedDigest =
                await cacheService
                    .HashGetAllAsync<LogDigestEntry>($"log-digest-queue:recipients:{recipient.HashKey}");
            
            var digest = 
               cachedDigest
                .Select(x => 
                    new KeyValuePair<Guid, LogDigestEntry>(
                        Guid.Parse(x.HashKey), 
                        x.Value))
                .ToDictionary();
            
            result.Add(recipient.HashKey, digest);
            
            // Delete used recipient
            await cacheService.HashDeleteAsync(
                "log-digest-queue:recipients-list",
                recipient.HashKey);
            
            await cacheService.DeleteAsync($"log-digest-queue:recipients:{recipient.HashKey}");
        }
        
        return result;
    }
}

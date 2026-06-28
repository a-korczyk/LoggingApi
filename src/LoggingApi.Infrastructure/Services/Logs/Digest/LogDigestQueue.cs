using LoggingApi.Application.Abstractions.Services;

namespace LoggingApi.Infrastructure.Services.Logs.Digest;

/// <summary>
/// Thread-safe in-memory implementation of <see cref="ILogDigestQueue"/>.
/// </summary>
public sealed class LogDigestQueue : ILogDigestQueue
{
    private readonly Lock _lock = new();
    
    /// <summary>
    /// Stores the recipients' emails and their <see cref="LogDigestEntry"/>s.
    /// </summary>
    private readonly Dictionary<string, Dictionary<Guid, LogDigestEntry>> _recipients = [];
    
    public void Insert(
        string email,
        LogDigestEntry entry)
    {
        lock (_lock)
        {
            if (_recipients.TryGetValue(email, out var recipient))
                recipient[entry.Id]  = entry;
            else
            {
                _recipients[email] = new();
                _recipients[email][entry.Id]  = entry;
            }
        }
    }

    public void Update(
        string email,
        LogDigestEntry entry)
    {
        lock (_lock)
        {
            if (!_recipients.TryGetValue(email, out var recipient))
                return;
            
            if (recipient.ContainsKey(entry.Id))
                recipient[entry.Id] = entry;
        }
    }

    public void Delete(
        string email,
        Guid id)
    {
        lock (_lock)
        {
            if (_recipients.TryGetValue(email, out var recipient)) 
                recipient.Remove(id);
        }
    }

    public IReadOnlyDictionary<string, IReadOnlyDictionary<Guid, LogDigestEntry>> TakeRecipients()
    {
        lock (_lock)
        {
            var recipients = 
                _recipients.ToDictionary(
                    pair => pair.Key,
                    pair => (IReadOnlyDictionary<Guid, LogDigestEntry>) pair.Value);
            
            _recipients.Clear();
            
            return recipients;
        }
    }
}

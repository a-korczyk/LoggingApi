using LoggingApi.Domain.Entities;

namespace LoggingApi.Application.Abstractions.Services;

/// <summary>
/// Stores <see cref="LogDigestEntry"/>s grouped by recipient for later inclusion in digest emails.
/// Provides methods for manipulating and retrieving the entry queue.
/// </summary>
public interface ILogDigestQueue
{
    /// <summary>
    /// Adds a new entry to the recipient's digest queue.
    /// </summary>
    /// <param name="email">Recipient's email.</param>
    /// <param name="entry">Entry to add.</param>
    public void Insert(
        string email,
        LogDigestEntry entry);

    /// <summary>
    /// Overwrites an existing entry of the same identifier.
    /// </summary>
    /// <param name="email">Recipient's email.</param>
    /// <param name="entry">The updated entry.</param>
    public void Update(
        string email,
        LogDigestEntry entry);

    /// <summary>
    /// Deletes the existing entry with the provided identifier.
    /// </summary>
    /// <param name="email">Recipient's email.</param>
    /// <param name="id">Entry's identifier.</param>
    public void Delete(
        string email,
        Guid id);

    /// <summary>
    /// Returns recipients and their digest entries, then clears it.
    /// </summary>
    /// <returns>
    /// A read-only dictionary, where the key is the recipient's email and the value
    /// is a read-only dictionary where the key is the log's identifier and the value is
    /// a <see cref="LogDigestEntry"/>.
    /// </returns>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<Guid, LogDigestEntry>> TakeRecipients();
}

/// <summary>
/// Represents a log entry in a recipient's digest queue.
/// </summary>
/// <param name="Id">Identifier of the log.</param>
/// <param name="Status">The log's status.</param>
/// <param name="Type">The log's type.</param>
/// <param name="Title">The log's title.</param>
public sealed record LogDigestEntry(
    Guid Id,
    LogStatus Status,
    LogType Type,
    string Title);
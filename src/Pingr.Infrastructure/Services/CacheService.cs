using System.Text.Json;
using StackExchange.Redis;

namespace Pingr.Infrastructure.Services;

/// <summary>
/// Reveals methods for communication with Redis.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves the value associated with the key.
    /// </summary>
    /// <param name="key">Key whose value to get.</param>
    Task<CacheResult<T?>> GetAsync<T>(string key);
    
    /// <summary>
    /// Sets the key with the provided value and/or expiration.
    /// </summary>
    /// <param name="expiration">Optional, defines in how much time the key
    /// will be deleted.</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration);
    
    /// <see cref="SetAsync"/>
    Task<bool> SetIfNotExistsAsync<T>(
        string key,
        T value,
        TimeSpan? expiration);
    
    /// <summary>
    /// Deletes the provided key.
    /// </summary>
    Task DeleteAsync(string key);

    /// <summary>
    /// Increments the value at the provided key.
    /// </summary>
    Task<long> IncrementAsync(string key);

    /// <summary>
    /// Increments the value at the provided key.
    /// If it's the first incrementation, then the provided
    /// expiration time will also be added to that key.
    /// </summary>
    /// <param name="expiration">In how much time the key will expire.</param>
    Task<long> IncrementWithExpirationAsync(
        string key,
        TimeSpan expiration);
    
    /// <summary>
    /// Gets all the hashes associated with the provided key.
    /// </summary>
    Task<IList<CacheHashEntry<T>>> HashGetAllAsync<T>(string key);

    /// <summary>
    /// Sets a hash for the associated key.
    /// </summary>
    Task HashSetAsync<T>(
        string key,
        string hashKey,
        T value);

    /// <see cref="HashSetAsync{T}"/>
    Task HashSetIfExistsAsync<T>(
        string key,
        string hashKey,
        T value);
    
    /// <see cref="HashSetAsync{T}"/>
    Task HashSetIfNotExistsAsync<T>(
        string key,
        string hashKey,
        T value);

    /// <summary>
    /// Removes a hash from the key.
    /// </summary>
    Task<bool> HashDeleteAsync(
        string key,
        string hashKey);
}

/// <inheritdoc/>
public sealed class CacheService(
    IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    
    public async Task<CacheResult<T?>> GetAsync<T>(string key) 
    {
        var value = await _database.StringGetAsync(key);

        if (!value.HasValue)
            return new CacheResult<T>(false, default);

        try
        {
            return new CacheResult<T?>(true, JsonSerializer.Deserialize<T>(value.ToString()));
        }
        catch (JsonException)
        {
            await _database.KeyDeleteAsync(key);
            return new CacheResult<T?>(false, default);
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);

        await _database.StringSetAsync(
            key,
            json,
            expiration,
            When.Always);
    }

    public async Task<bool> SetIfNotExistsAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);

        return await _database.StringSetAsync(
            key,
            json,
            expiration,
            When.NotExists);
    }

    public async Task DeleteAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task<long> IncrementAsync(string key)
    {
        return await _database.StringIncrementAsync(key);
    }

    public async Task<long> IncrementWithExpirationAsync(
        string key,
        TimeSpan expiration)
    {
        var count = await _database.StringIncrementAsync(key);
        
        if (count is 1)
            await _database.KeyExpireAsync(key, expiration);

        return count;
    }

    public async Task<IList<CacheHashEntry<T>>> HashGetAllAsync<T>(string key)
    {
        var entries = await _database.HashGetAllAsync(key);

        if (!entries.Any())
            return [];

        var result = new List<CacheHashEntry<T>>();

        foreach (var entry in entries)
        {
            try
            {
                result.Add(new(
                    entry.Name.ToString(),
                    JsonSerializer.Deserialize<T>(entry.Value.ToString())));
            }
            catch (JsonException)
            {
                await _database.HashDeleteAsync(key, entry.Name);
            }
        }
        
        return result;
    }

    public async Task HashSetAsync<T>(
        string key,
        string hashKey,
        T value)
    {
        var json = JsonSerializer.Serialize(value);
        
        await _database.HashSetAsync(
            key,
            hashKey,
            json);
    }
    
    public async Task HashSetIfExistsAsync<T>(
        string key,
        string hashKey,
        T value)
    {
        var json = JsonSerializer.Serialize(value);
        
        await _database.HashSetAsync(
            key,
            hashKey,
            json,
            When.Exists);
    }

    public async Task HashSetIfNotExistsAsync<T>(
        string key,
        string hashKey,
        T value)
    {
        var json = JsonSerializer.Serialize(value);
        
        await _database.HashSetAsync(
            key,
            hashKey,
            json,
            When.NotExists);
    }

    public async Task<bool> HashDeleteAsync(
        string key,
        string hashKey)
    {
        return await _database.HashDeleteAsync(key, hashKey);
    }
}

public sealed record CacheResult<T>(
    bool Exists,
    T? Value);
    
public sealed record CacheHashEntry<T>(
    string HashKey,
    T? Value);
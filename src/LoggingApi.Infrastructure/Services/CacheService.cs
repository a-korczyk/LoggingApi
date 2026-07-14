using System.Text.Json;
using StackExchange.Redis;

namespace LoggingApi.Infrastructure.Services;

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
    
    /// <seealso cref="SetAsync"/>
    Task<bool> SetIfNotExistsAsync<T>(
        string key,
        T value,
        TimeSpan? expiration);
    
    Task DeleteAsync(string key);

    Task<long> IncrementAsync(string key);

    Task<long> IncrementWithExpirationAsync(
        string key,
        TimeSpan expiration);
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
}

public sealed record CacheResult<T>(
    bool Exists,
    T? Value);
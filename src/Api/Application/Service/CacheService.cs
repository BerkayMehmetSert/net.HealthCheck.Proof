using System.Text.Json;
using StackExchange.Redis;

namespace Api.Application.Service;

public class CacheService : ICacheService
{
    private readonly IDatabase _db;

    public CacheService(ICacheDatabaseProvider cacheDatabaseProvider)
    {
        _db = cacheDatabaseProvider.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var result = await _db.StringGetAsync(key);

        return !result.HasValue ? default : JsonSerializer.Deserialize<T>(result);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expires = null,
        When when = When.Always,
        CommandFlags flags = CommandFlags.None)
    {
        var json = JsonSerializer.Serialize(value);
        return await _db.StringSetAsync(key, json, expires, when, flags);
    }
}
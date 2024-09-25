using StackExchange.Redis;

namespace Api.Application.Service;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expires = default, When when = When.Always, CommandFlags flags = CommandFlags.None);
}
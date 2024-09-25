using StackExchange.Redis;

namespace Api.Application.Service;

public interface ICacheDatabaseProvider
{
    IDatabase GetDatabase();
}
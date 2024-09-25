using Api.Application.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Api.Application.Service;

public class CacheDatabaseProvider : ICacheDatabaseProvider
{
    private readonly ILogger<CacheDatabaseProvider> _logger;
    private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
    private readonly RedisSettings _redisSettings;

    public CacheDatabaseProvider(ILogger<CacheDatabaseProvider> logger,
        IOptions<RedisSettings> redisSettings,
        Func<ConfigurationOptions, ConnectionMultiplexer> connectionFactory = null)
    {
        _logger = logger;
        _redisSettings = redisSettings.Value;

        _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(() =>
            (connectionFactory ?? DefaultConnectionMultiplexerFactory)(GetConfigurationOptions()));
    }

    public IDatabase GetDatabase()
    {
        return _connectionMultiplexer.Value.GetDatabase();
    }

    private static ConnectionMultiplexer DefaultConnectionMultiplexerFactory(ConfigurationOptions options)
    {
        return ConnectionMultiplexer.Connect(options);
    }

    private ConfigurationOptions GetConfigurationOptions()
    {
        var options = new ConfigurationOptions
        {
            Password = _redisSettings.Password,
            AllowAdmin = true,
            DefaultDatabase = _redisSettings.DefaultDatabase,
            KeepAlive = 60,
            SyncTimeout = _redisSettings.SyncTimeout,
            ConnectTimeout = _redisSettings.ConnectTimeout
        };

        var endPoints = _redisSettings.EndPoint.Split(",");
        foreach (var endPoint in endPoints)
        {
            options.EndPoints.Add(endPoint);
        }

        return options;
    }
}
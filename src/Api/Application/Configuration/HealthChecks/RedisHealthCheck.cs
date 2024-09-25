using Api.Application.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Api.Application.Configuration.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly RedisSettings _redisSettings;

    public RedisHealthCheck(RedisSettings redisSettings) => _redisSettings = redisSettings;


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var config = ConfigurationOptions.Parse(_redisSettings.EndPoint);
            if (!string.IsNullOrWhiteSpace(_redisSettings.Password))
            {
                config.Password = _redisSettings.Password;
            }

            using var connection = await ConnectionMultiplexer.ConnectAsync(config);
            return connection.IsConnected
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("Unable to connect to Redis.");
        }
        catch (Exception e)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: e);
        }
    }

}
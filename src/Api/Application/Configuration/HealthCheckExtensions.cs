using Api.Application.Configuration.HealthChecks;
using Api.Application.Settings;
using Api.Integration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Application.Configuration;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddDatabase(this IHealthChecksBuilder builder, string connectionString,
        string? name, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(name ?? string.Empty, sp => new DatabaseHealthCheck(
                connectionString ?? throw new Exception($"{nameof(connectionString)} parameter is required")),
            failureStatus, tags, timeout));
    }

    public static IHealthChecksBuilder AddRedisHealthCheck(this IHealthChecksBuilder builder,
        RedisSettings redisSettings)
    {
        return builder.AddCheck<RedisHealthCheck>("redis", failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "redis" });
    }

    public static IHealthChecksBuilder AddJsonPlaceholder(this IHealthChecksBuilder builder, string? name,
        HealthStatus? failureStatus = default, IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(name ?? string.Empty, sp => new JsonPlaceholderHealthCheck(
                sp.GetRequiredService<IJsonPlaceholderApi>()),
            failureStatus, tags, timeout));
    }
}
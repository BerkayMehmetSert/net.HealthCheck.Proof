using Api.Application.Settings;

namespace Api.Application.Configuration;

public static class HealthCheckConfiguration
{
    public static void ConfigureHealthChecks(this IServiceCollection services, string connectionString, string healthCheckDbConnectionString, RedisSettings redisSettings)
    {
        services.AddHealthChecks()
            .AddDatabase(connectionString, "database")
            .AddJsonPlaceholder("jsonplaceholder")
            .AddRedisHealthCheck(redisSettings);

        services.AddHealthChecksUI(setupSettings: setup =>
            {
                try
                {
                    setup.UseApiEndpointHttpMessageHandler(sp =>
                    {
                        return new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                        };
                    });

                    setup.MaximumHistoryEntriesPerEndpoint(500);
                    setup.AddHealthCheckEndpoint("Project", "https://localhost:44315" + "/health");
                    setup.SetEvaluationTimeInSeconds(30);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HealthCheckUI setup error: " + ex.Message);
                }
            })
            .AddPostgreSqlStorage(healthCheckDbConnectionString);
    }
}
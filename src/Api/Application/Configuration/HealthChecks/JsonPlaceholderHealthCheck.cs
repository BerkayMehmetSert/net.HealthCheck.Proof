using Api.Integration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Application.Configuration.HealthChecks;

public class JsonPlaceholderHealthCheck : IHealthCheck
{
    private readonly IJsonPlaceholderApi _jsonPlaceholderApi;

    public JsonPlaceholderHealthCheck(IJsonPlaceholderApi jsonPlaceholderApi)
    {
        _jsonPlaceholderApi = jsonPlaceholderApi;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var response = await _jsonPlaceholderApi.GetPosts();
            if (response.Count > 0)
            {
                return HealthCheckResult.Healthy();
            }

            return new HealthCheckResult(context.Registration.FailureStatus,
                description:
                "JsonPlaceholder API is not responding with success status code or returned an empty list.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new HealthCheckResult(context.Registration.FailureStatus, exception: new Exception(e.Message));
        }
    }
}
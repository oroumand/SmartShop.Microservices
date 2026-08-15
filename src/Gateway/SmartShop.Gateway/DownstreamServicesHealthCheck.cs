using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SmartShop.Gateway;

public sealed class DownstreamServicesHealthCheck(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var urls = configuration
            .GetSection("HealthChecks:Downstream")
            .Get<string[]>() ?? [];

        if (urls.Length == 0)
        {
            return HealthCheckResult.Unhealthy("No downstream health URLs were configured.");
        }

        var client = httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(2);

        foreach (var url in urls)
        {
            try
            {
                using var response = await client.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Unhealthy(
                        $"Downstream '{url}' returned {(int)response.StatusCode}.");
                }
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                return HealthCheckResult.Unhealthy(
                    $"Downstream '{url}' is unavailable.",
                    exception);
            }
        }

        return HealthCheckResult.Healthy("All configured downstream services are reachable.");
    }
}

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SmartShop.Gateway;
using SmartShop.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient();
builder.Services.AddHealthChecks()
    .AddCheck<DownstreamServicesHealthCheck>(
        "downstream-services",
        tags: ["ready"]);

var app = builder.Build();

app.UseCorrelationId();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});
app.MapReverseProxy();

app.Run();

public partial class Program;

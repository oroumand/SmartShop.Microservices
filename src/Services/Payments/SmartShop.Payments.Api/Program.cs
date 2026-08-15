using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using SmartShop.Messaging.RabbitMq;
using SmartShop.ModuleContracts.Ordering;
using SmartShop.Payments.Api;
using SmartShop.Payments.Api.Ordering;
using SmartShop.Payments.Endpoints;
using SmartShop.Payments.Infra.Data;
using SmartShop.Payments.Infra.Data.Database;
using SmartShop.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddPaymentsData(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration, "payments-service");
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdPropagationHandler>();

builder.Services
    .AddHttpClient<IOrderingPaymentContract, HttpOrderingPaymentContract>((services, client) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var baseUrl = configuration["Services:Ordering:BaseUrl"]
            ?? throw new InvalidOperationException("Services:Ordering:BaseUrl was not configured.");

        client.BaseAddress = new Uri(baseUrl);
    })
    .AddHttpMessageHandler<CorrelationIdPropagationHandler>()
    .AddStandardResilienceHandler(options =>
    {
        options.TotalRequestTimeout.Timeout = TimeSpan.FromMilliseconds(800);
        options.AttemptTimeout.Timeout = TimeSpan.FromMilliseconds(500);
        options.Retry.MaxRetryAttempts = 2;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
    });

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PaymentsDbContext>("payments-database");

var app = builder.Build();

app.UseCorrelationId();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<PaymentsDatabaseInitializer>();

    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SmartShop Payments API");
    });
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");
app.MapPaymentsEndpoints();
app.MapOutboxDiagnosticsEndpoints();

app.Run();

public partial class Program;

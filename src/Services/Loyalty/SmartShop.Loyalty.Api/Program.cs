using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using SmartShop.Loyalty.Api;
using SmartShop.Loyalty.Api.Consumers;
using SmartShop.Loyalty.Infra.Data;
using SmartShop.Messaging.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddLoyaltyData(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration, "loyalty-service");
builder.Services.AddHostedService<PaymentSucceededConsumer>();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LoyaltyDbContext>("loyalty-database");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<LoyaltyDatabaseInitializer>();

    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SmartShop Loyalty API");
    });
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");
app.MapLoyaltyEndpoints();

app.Run();

public partial class Program;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using SmartShop.AiSearch.Core.Application.Search;
using SmartShop.AiSearch.Endpoints;
using SmartShop.AiSearch.Infra.OpenAI;
using SmartShop.AiSearch.Infra.Qdrant;
using SmartShop.Catalog.Endpoints;
using SmartShop.Catalog.Infra.Data;
using SmartShop.Ordering.Endpoints;
using SmartShop.Ordering.Infra.Data;
using SmartShop.Ordering.Infra.Data.Database;
using SmartShop.Messaging.RabbitMq;
using SmartShop.Api.Consumers;
using SmartShop.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCatalogData(builder.Configuration);
builder.Services.AddOrderingData(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration, "smartshop-monolith");
builder.Services.AddHostedService<OrderingPaymentSucceededConsumer>();
builder.Services.AddOpenAiEmbeddings(builder.Configuration);
builder.Services.AddQdrantVectorStore(builder.Configuration);
builder.Services.AddScoped<IAiSearchIndexingService, AiSearchIndexingService>();
builder.Services.AddScoped<IAiSearchQueryService, AiSearchQueryService>();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatalogDbContext>("catalog-database")
    .AddDbContextCheck<OrderingDbContext>("ordering-database");

var app = builder.Build();

app.UseCorrelationId();

using (var scope = app.Services.CreateScope())
{
    var catalogDatabaseInitializer =
        scope.ServiceProvider.GetRequiredService<CatalogDatabaseInitializer>();

    await catalogDatabaseInitializer.InitializeAsync();

    var orderingDatabaseInitializer =
        scope.ServiceProvider.GetRequiredService<OrderingDatabaseInitializer>();

    await orderingDatabaseInitializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SmartShop API");
    });
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");
app.MapCatalogEndpoints();
app.MapOrderingEndpoints();
app.MapAiSearchEndpoints();

app.Run();

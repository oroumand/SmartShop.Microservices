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
using SmartShop.Payments.Endpoints;
using SmartShop.Payments.Infra.Data;
using SmartShop.Payments.Infra.Data.Database;
using SmartShop.Messaging.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCatalogData(builder.Configuration);
builder.Services.AddOrderingData(builder.Configuration);
builder.Services.AddPaymentsData(builder.Configuration);
builder.Services.AddRabbitMqPublisher(builder.Configuration, "smartshop-monolith");
builder.Services.AddOpenAiEmbeddings(builder.Configuration);
builder.Services.AddQdrantVectorStore(builder.Configuration);
builder.Services.AddScoped<IAiSearchIndexingService, AiSearchIndexingService>();
builder.Services.AddScoped<IAiSearchQueryService, AiSearchQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var catalogDatabaseInitializer =
        scope.ServiceProvider.GetRequiredService<CatalogDatabaseInitializer>();

    await catalogDatabaseInitializer.InitializeAsync();

    var orderingDatabaseInitializer =
        scope.ServiceProvider.GetRequiredService<OrderingDatabaseInitializer>();

    await orderingDatabaseInitializer.InitializeAsync();

    var paymentsDatabaseInitializer =
        scope.ServiceProvider.GetRequiredService<PaymentsDatabaseInitializer>();

    await paymentsDatabaseInitializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("SmartShop API");
    });
}

app.MapGet("/health", () => "OK");
app.MapCatalogEndpoints();
app.MapOrderingEndpoints();
app.MapPaymentsEndpoints();
app.MapAiSearchEndpoints();

app.Run();

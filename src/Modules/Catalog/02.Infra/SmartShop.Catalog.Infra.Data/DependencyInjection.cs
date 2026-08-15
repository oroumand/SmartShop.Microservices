using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartShop.Catalog.Core.Application.Products;
using SmartShop.Catalog.Infra.Data.Products;
using SmartShop.ModuleContracts.Catalog;

namespace SmartShop.Catalog.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmartShopDb")
            ?? throw new InvalidOperationException(
                "Connection string 'SmartShopDb' was not found.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductQueryService, EfProductQueryService>();
        services.AddScoped<ICatalogProductLookup, EfCatalogProductLookup>();
        services.AddScoped<ICatalogProductIndexSource, EfCatalogProductIndexSource>();
        services.AddScoped<CatalogDatabaseInitializer>();

        return services;
    }
}

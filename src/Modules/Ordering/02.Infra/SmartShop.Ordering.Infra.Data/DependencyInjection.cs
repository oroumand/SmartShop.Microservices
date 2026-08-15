using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartShop.ModuleContracts.Ordering;
using SmartShop.Ordering.Core.Application.Orders;
using SmartShop.Ordering.Infra.Data.Database;
using SmartShop.Ordering.Infra.Data.Orders;

namespace SmartShop.Ordering.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmartShopDb")
            ?? throw new InvalidOperationException(
                "Connection string 'SmartShopDb' was not found.");

        services.AddDbContext<OrderingDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IOrderRepository, EfOrderRepository>();
        services.AddScoped<IOrderCommandService, CreateOrderService>();
        services.AddScoped<IOrderQueryService, EfOrderQueryService>();
        services.AddScoped<IOrderingPaymentContract, EfOrderingPaymentContract>();
        services.AddScoped<OrderingDatabaseInitializer>();

        return services;
    }
}

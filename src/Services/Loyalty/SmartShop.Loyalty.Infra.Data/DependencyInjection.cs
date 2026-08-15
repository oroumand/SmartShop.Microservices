using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartShop.Loyalty.Core.Application.Accounts;
using SmartShop.Loyalty.Infra.Data.Accounts;

namespace SmartShop.Loyalty.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddLoyaltyData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("LoyaltyDb")
            ?? throw new InvalidOperationException(
                "Connection string 'LoyaltyDb' was not found.");

        services.AddDbContext<LoyaltyDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ILoyaltyAccountQueryService, EfLoyaltyAccountQueryService>();
        services.AddScoped<IEarnLoyaltyPointsService, EfEarnLoyaltyPointsService>();
        services.AddScoped<LoyaltyDatabaseInitializer>();

        return services;
    }
}

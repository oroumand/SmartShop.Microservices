using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartShop.Payments.Core.Application.Payments;
using SmartShop.Payments.Infra.Data.Database;
using SmartShop.Payments.Infra.Data.Payments;
using SmartShop.IntegrationEvents;
using SmartShop.Payments.Infra.Data.Outbox;

namespace SmartShop.Payments.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentsDb")
            ?? throw new InvalidOperationException(
                "Connection string 'PaymentsDb' was not found.");

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPaymentRepository, EfPaymentRepository>();
        services.AddScoped<IIntegrationEventOutbox, EfIntegrationEventOutbox>();
        services.AddScoped<IPaymentCommandService, PayOrderService>();
        services.AddScoped<IPaymentQueryService, EfPaymentQueryService>();
        services.AddScoped<PaymentsDatabaseInitializer>();
        services.AddHostedService<PaymentOutboxPublisher>();

        return services;
    }
}

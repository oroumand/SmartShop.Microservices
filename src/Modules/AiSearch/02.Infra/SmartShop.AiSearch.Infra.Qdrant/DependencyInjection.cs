using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartShop.AiSearch.Core.Application.Search;

namespace SmartShop.AiSearch.Infra.Qdrant;

public static class DependencyInjection
{
    public static IServiceCollection AddQdrantVectorStore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<QdrantOptions>(
            configuration.GetSection(QdrantOptions.SectionName));

        services.AddHttpClient<QdrantProductVectorStore>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<QdrantOptions>>()
                .Value;

            httpClient.BaseAddress = new Uri(options.BaseUrl);

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
            }
        });

        services.AddScoped<IProductVectorStore>(serviceProvider =>
            serviceProvider.GetRequiredService<QdrantProductVectorStore>());

        return services;
    }
}

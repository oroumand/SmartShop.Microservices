using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartShop.AiSearch.Core.Application.Search;

namespace SmartShop.AiSearch.Infra.OpenAI;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenAiEmbeddings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpenAiEmbeddingOptions>(
            configuration.GetSection(OpenAiEmbeddingOptions.SectionName));

        services.AddHttpClient<OpenAiTextEmbeddingGenerator>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<OpenAiEmbeddingOptions>>()
                .Value;

            httpClient.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddScoped<ITextEmbeddingGenerator>(serviceProvider =>
            serviceProvider.GetRequiredService<OpenAiTextEmbeddingGenerator>());

        return services;
    }
}

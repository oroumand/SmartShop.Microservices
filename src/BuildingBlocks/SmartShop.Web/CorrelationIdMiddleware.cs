using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace SmartShop.Web;

public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);

        context.TraceIdentifier = correlationId;
        context.Request.Headers[HeaderName] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            var startedAt = Stopwatch.GetTimestamp();
            logger.LogInformation(
                "HTTP {Method} {Path} started with correlation {CorrelationId}.",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            try
            {
                await next(context);
            }
            finally
            {
                logger.LogInformation(
                    "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds} ms and correlation {CorrelationId}.",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                    correlationId);
            }
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out StringValues values))
        {
            var candidate = values.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(candidate) && candidate.Length <= 128)
            {
                return candidate;
            }
        }

        return Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
    }
}

public sealed class CorrelationIdPropagationHandler(IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?
            .Request.Headers[CorrelationIdMiddleware.HeaderName]
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.Remove(CorrelationIdMiddleware.HeaderName);
            request.Headers.TryAddWithoutValidation(
                CorrelationIdMiddleware.HeaderName,
                correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

public static class CorrelationIdExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();
}

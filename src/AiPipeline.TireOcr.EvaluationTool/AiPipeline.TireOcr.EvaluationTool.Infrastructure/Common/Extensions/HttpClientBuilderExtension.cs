using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Extensions;

public static class HttpClientBuilderExtension
{
    public static IHttpClientBuilder RemoveResilienceHandlers(this IHttpClientBuilder builder)
    {
        builder.ConfigureAdditionalHttpMessageHandlers(static (handlers, _) =>
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i] is ResilienceHandler)
                {
                    handlers.RemoveAt(i);
                }
            }
        });
        return builder;
    }

    public static IHttpClientBuilder ApplyCustomResilienceHandler(this IHttpClientBuilder builder, int timeoutSeconds)
    {
        builder
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromSeconds(timeoutSeconds);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
        return builder;
    }
}
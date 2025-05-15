using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace TireOcr.ServiceDefaults;

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
}
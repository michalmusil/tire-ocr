using System.Text.Json.Serialization;
using AiPipeline.TireOcr.EvaluationTool.WebApi.Common.AuthenticationSchemas.UserOrApiKey;
using Asp.Versioning;
using Microsoft.OpenApi.Models;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        AddControllersConfig(services);
        AddRoutesConfig(services);
        AddJwt(services, configuration);
        AddSwagger(services);
        return services;
    }

    private static void AddControllersConfig(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters
                .Add(new JsonStringEnumConverter())
            );
    }

    private static void AddRoutesConfig(IServiceCollection serviceCollection)
    {
        serviceCollection.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    public static void AddJwt(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = UserOrApiKeyAuthenticationOptions.SchemeName;
                options.DefaultChallengeScheme = UserOrApiKeyAuthenticationOptions.SchemeName;
                options.DefaultScheme = UserOrApiKeyAuthenticationOptions.SchemeName;
            })
            .AddScheme<UserOrApiKeyAuthenticationOptions, UserOrApiKeyAuthenticationHandler>(
                UserOrApiKeyAuthenticationOptions.SchemeName, options => { }
            );
        serviceCollection.AddAuthorization();
    }

    private static void AddSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(opts =>
        {
            opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            opts.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
    }
}
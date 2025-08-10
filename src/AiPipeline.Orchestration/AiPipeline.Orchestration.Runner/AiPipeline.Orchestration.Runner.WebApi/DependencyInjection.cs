using System.Text;
using System.Text.Json.Serialization;
using AiPipeline.Orchestration.Runner.Infrastructure.User.Dtos;
using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Converters;
using AiPipeline.Orchestration.Shared.All.Extensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.Orchestration.Runner.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration, IHostBuilder hostBuilder)
    {
        AddControllersConfig(services);
        AddRoutesConfig(services);
        AddJwt(services, configuration);
        AddSwagger(services);
        AddMessaging(hostBuilder);
        return services;
    }

    private static void AddControllersConfig(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new ApElementConverter());
            });
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
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtOptions = new JwtOptions();
            configuration.GetSection(JwtOptions.Key).Bind(jwtOptions);
            if (!jwtOptions.IsValid)
                throw new InvalidOperationException("Failed to bind JwtOptions");

            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
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

    private static void AddMessaging(IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine(opt =>
        {
            opt.UseRabbitMqUsingNamedConnection("rabbitmq")
                .DeclareExchanges()
                .AutoProvision();

            opt.ConfigureMessagePublishing();
            opt.ApplyCustomConfiguration();

            opt.ListenToRabbitQueue(MessagingConstants.AdvertisementsQueueName);
            opt.ListenToRabbitQueue(MessagingConstants.CompletedPipelineStepsQueueName);
            opt.ListenToRabbitQueue(MessagingConstants.FailedPipelinesQueueName);
        });
    }
}
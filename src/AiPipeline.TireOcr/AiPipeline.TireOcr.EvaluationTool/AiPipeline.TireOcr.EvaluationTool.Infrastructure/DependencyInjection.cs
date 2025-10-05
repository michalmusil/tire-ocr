using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Extensions;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Facades;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.ProcessorMappers;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.DbMatching;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Ocr;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Postprocessing;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Preprocessing;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddServices(services);
        AddFacades(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IContentTypeValidationService, StaticContentTypeValidationService>();
        services.AddScoped<ITireCodeSimilarityEvaluationService, TireCodeSimilarityEvaluationService>();
        services.AddScoped<ICsvParserService, CsvParserService>();
        services.AddHttpClient<IImageDownloadService, ImageDownloaderService>();

        services.AddScoped<PreprocessingNoneProcessor>();
        services.AddHttpClient<PreprocessingRoiExtractionProcessor>(client =>
            {
                client.BaseAddress = new("https+http://PreprocessingService");
            })
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromSeconds(120);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
        services.AddHttpClient<PreprocessingResizeProcessor>(client =>
            {
                client.BaseAddress = new("https+http://PreprocessingService");
            })
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromSeconds(60);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
        services.AddHttpClient<OcrRemoteServicesProcessor>(client =>
            {
                client.BaseAddress = new("https+http://OcrService");
            })
            .RemoveResilienceHandlers()
            .AddStandardResilienceHandler(opt =>
            {
                var timeout = TimeSpan.FromSeconds(90);
                opt.AttemptTimeout.Timeout = timeout;
                opt.TotalRequestTimeout.Timeout = timeout;
                opt.CircuitBreaker.SamplingDuration = 2 * timeout;
            });
        services.AddHttpClient<PostprocessingRemoteServiceProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PostprocessingService");
        });
        services.AddScoped<DbMatchingNoneProcessor>();
        services.AddHttpClient<DbMatchingRemoteProcessor>(client =>
        {
            client.BaseAddress = new("https+http://TasyDbMatcherService");
        });

        services
            .AddScoped<IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor>, PreprocessingProcessorMapper>();
        services.AddScoped<IEnumToObjectMapper<OcrType, IOcrProcessor>, OcrProcessorMapper>();
        services
            .AddScoped<IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor>,
                PostprocessingProcessorMapper>();
        services.AddScoped<IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor>, DbMatchingProcessorMapper>();
    }

    private static void AddFacades(IServiceCollection services)
    {
        services.AddScoped<IRunFacade, RunFacade>();
    }
}
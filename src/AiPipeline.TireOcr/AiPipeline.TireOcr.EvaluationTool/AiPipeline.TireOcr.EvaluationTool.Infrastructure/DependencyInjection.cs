using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Application.User.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Extensions;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Services;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Facades;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.ProcessorMappers;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.DbMatching;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.Ocr;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.Postprocessing;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.Preprocessing;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Repositories;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddUnitOfWork(services);
        AddServices(services);
        AddExternalServices(services);
        AddFacades(services);
        AddDbContext(services, configuration);
        return services;
    }

    public static void AddUnitOfWork(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEvaluationRunEntityRepository, EvaluationRunEntityRepository>();
        serviceCollection.AddScoped<IEvaluationRunBatchEntityRepository, EvaluationRunBatchEntityRepository>();
        serviceCollection.AddScoped<IUserEntityRepository, UserEntityRepository>();
        serviceCollection.AddScoped<IRefreshTokenEntityRepository, RefreshTokenEntityRepository>();
        serviceCollection.AddScoped<IApiKeyRepository, ApiKeyRepository>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IContentTypeValidationService, StaticContentTypeValidationService>();
        services.AddScoped<ITireCodeSimilarityEvaluationService, TireCodeSimilarityEvaluationService>();
        services.AddScoped<ICsvParserService, CsvParserService>();
        services.AddScoped<IBatchCsvExportService, BatchCsvExportService>();
        services.AddScoped<IBatchEvaluationService, BatchEvaluationService>();
        services.AddHttpClient<IImageDownloadService, ImageDownloaderService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IHashService, HashService>();
        services.AddSingleton<ICryptographyService, CryptographyService>();

        services
            .AddScoped<IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor>, PreprocessingProcessorMapper>();
        services.AddScoped<IEnumToObjectMapper<OcrType, IOcrProcessor>, OcrProcessorMapper>();
        services
            .AddScoped<IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor>,
                PostprocessingProcessorMapper>();
        services.AddScoped<IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor>, DbMatchingProcessorMapper>();
    }

    private static void AddExternalServices(IServiceCollection services)
    {
        var preprocessingTimeoutSeconds = 60;
        var ocrTimeoutSeconds = 90;

        // Preprocessing

        services.AddHttpClient<PreprocessingRoiExtractionProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PreprocessingService");
        }).ApplyCustomResilienceHandler(preprocessingTimeoutSeconds);

        services.AddHttpClient<PreprocessingResizeProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PreprocessingService");
        }).ApplyCustomResilienceHandler(preprocessingTimeoutSeconds);

        services.AddHttpClient<PreprocessingSlicesCompositionProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PreprocessingService");
        }).ApplyCustomResilienceHandler(preprocessingTimeoutSeconds);

        services.AddHttpClient<PreprocessingTextExtractionMosaicProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PreprocessingPythonService");
        }).ApplyCustomResilienceHandler(preprocessingTimeoutSeconds);

        services.AddHttpClient<PreprocessingSlicesEnhanceTextProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PreprocessingPythonService");
        }).ApplyCustomResilienceHandler(preprocessingTimeoutSeconds);

        // OCR

        services.AddHttpClient<OcrRemoteServicesProcessor>(client =>
        {
            client.BaseAddress = new("https+http://OcrService");
        }).ApplyCustomResilienceHandler(ocrTimeoutSeconds);

        services.AddHttpClient<OcrRemotePythonProcessor>(client =>
        {
            client.BaseAddress = new("https+http://OcrPythonService");
        }).ApplyCustomResilienceHandler(ocrTimeoutSeconds);

        // Postprocessing

        services.AddHttpClient<PostprocessingRemoteServiceProcessor>(client =>
        {
            client.BaseAddress = new("https+http://PostprocessingService");
        });

        services.AddScoped<DbMatchingNoneProcessor>();
        
        // DbMatching

        services.AddHttpClient<DbMatchingRemoteProcessor>(client =>
        {
            client.BaseAddress = new("https+http://TasyDbMatcherService");
        });
    }

    private static void AddFacades(IServiceCollection services)
    {
        services.AddScoped<IRunFacade, RunFacade>();
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AiPipelineEvaluationToolDb");
        services.AddDbContextFactory<EvaluationToolDbContext>(options => { options.UseNpgsql(connectionString); });
    }
}
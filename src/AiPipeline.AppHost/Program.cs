using Projects;
using TireOcr.AppHost.CustomIntegrations.Minio;

var builder = DistributedApplication.CreateBuilder(args);

// var minio = builder.AddMinio("minio");
//
// var rabbitMq = builder
//     .AddRabbitMQ("rabbitmq")
//     .WithDataVolume()
//     .WithManagementPlugin(port: 15672);
//
// var fileService = builder
//     .AddProject<AiPipeline_Orchestration_FileService_GrpcServer>("FileService")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(minio)
//     .WithMinioCredentials(minio)
//     .WaitFor(minio);
//
// var orchestrationRunnerService = builder
//     .AddProject<AiPipeline_Orchestration_Runner_WebApi>("OrchestrationRunnerService")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(fileService)
//     .WithReference(rabbitMq)
//     .WaitFor(rabbitMq)
//     .WithReference(minio)
//     .WithMinioCredentials(minio)
//     .WaitFor(minio);

var preprocessingService = builder.AddProject<AiPipeline_TireOcr_Preprocessing_WebApi>("PreprocessingService")
    .WithHttpsHealthCheck("/health");

var ocrService = builder.AddProject<AiPipeline_TireOcr_Ocr_WebApi>("OcrService")
    .WithHttpsHealthCheck("/health");

// var preprocessingMessagingService = builder
//     .AddProject<AiPipeline_TireOcr_Preprocessing_Messaging>("PreprocessingMessagingService")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(rabbitMq)
//     .WaitFor(rabbitMq)
//     .WithReference(fileService);
//
// var ocrMessagingService = builder.AddProject<AiPipeline_TireOcr_Ocr_Messaging>("OcrMessagingService")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(rabbitMq)
//     .WaitFor(rabbitMq)
//     .WithReference(fileService);

var postprocessingService = builder.AddProject<AiPipeline_TireOcr_Postprocessing_WebApi>("PostprocessingService")
    .WithHttpsHealthCheck("/health");

var tireDbMatcherService = builder.AddProject<AiPipeline_TireOcr_TasyDbMatcher_WebApi>("TasyDbMatcherService")
    .WithHttpsHealthCheck("/health");

// var postprocessingMessagingService = builder
//     .AddProject<AiPipeline_TireOcr_Postprocessing_Messaging>("PostprocessingMessagingService")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(rabbitMq)
//     .WaitFor(rabbitMq);
//
// var dbMatcherMessagingService = builder
//     .AddProject<AiPipeline_TireOcr_TasyDbMatcher_Messaging>("TasyDbMatcherMessagingService")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(rabbitMq)
//     .WaitFor(rabbitMq);

var runnerPrototype = builder.AddProject<AiPipeline_TireOcr_RunnerPrototype>("RunnerPrototype")
    .WithHttpsHealthCheck("/health")
    .WithReference(preprocessingService)
    .WaitFor(preprocessingService)
    .WithReference(ocrService)
    .WaitFor(ocrService)
    .WithReference(postprocessingService)
    .WaitFor(postprocessingService)
    .WithReference(tireDbMatcherService)
    .WaitFor(tireDbMatcherService);

builder.Build().Run();
using Projects;
using TireOcr.AppHost.CustomIntegrations.Minio;

var builder = DistributedApplication.CreateBuilder(args);

var minio = builder.AddMinio("minio");

var rabbitMq = builder
    .AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithManagementPlugin(port: 15672);

var orchestrationRunnerService = builder
    .AddProject<AiPipeline_Orchestration_Runner_WebApi>("OrchestrationRunnerService")
    .WithHttpsHealthCheck("/health")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(minio)
    .WithMinioCredentials(minio)
    .WaitFor(minio);

// var preprocessingService = builder.AddProject<AiPipeline_TireOcr_Preprocessing_WebApi>("PreprocessingService")
//     .WithHttpsHealthCheck("/health");
//
// var ocrService = builder.AddProject<AiPipeline_TireOcr_Ocr_WebApi>("OcrService")
//     .WithHttpsHealthCheck("/health");

var preprocessingMessagingService = builder
    .AddProject<AiPipeline_TireOcr_Preprocessing_Messaging>("PreprocessingMessagingService")
    .WithHttpsHealthCheck("/health")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(orchestrationRunnerService)
    .WaitFor(orchestrationRunnerService);

var ocrMessagingService = builder.AddProject<AiPipeline_TireOcr_Ocr_Messaging>("OcrMessagingService")
    .WithHttpsHealthCheck("/health")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(orchestrationRunnerService)
    .WaitFor(orchestrationRunnerService);

// var postprocessingService = builder.AddProject<AiPipeline_TireOcr_Postprocessing_WebApi>("PostprocessingService")
//     .WithHttpsHealthCheck("/health");

var postprocessingMessagingService = builder
    .AddProject<AiPipeline_TireOcr_Postprocessing_Messaging>("PostprocessingMessagingService")
    .WithHttpsHealthCheck("/health")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

// var runnerPrototype = builder.AddProject<AiPipeline_TireOcr_RunnerPrototype>("RunnerPrototype")
//     .WithHttpsHealthCheck("/health")
//     .WithReference(preprocessingService)
//     .WaitFor(preprocessingService)
//     .WithReference(ocrService)
//     .WaitFor(ocrService)
//     .WithReference(postprocessingService)
//     .WaitFor(postprocessingService);

builder.Build().Run();
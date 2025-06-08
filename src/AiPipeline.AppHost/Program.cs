using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var preprocessingService = builder.AddProject<AiPipeline_TireOcr_Preprocessing_WebApi>("PreprocessingService")
    .WithHttpsHealthCheck("/health");

var ocrService = builder.AddProject<AiPipeline_TireOcr_Ocr_WebApi>("OcrService")
    .WithHttpsHealthCheck("/health");

var postprocessingService = builder.AddProject<AiPipeline_TireOcr_Postprocessing_WebApi>("PostprocessingService")
    .WithHttpsHealthCheck("/health");

var orchestrationRunnerService = builder.AddProject<AiPipeline_Orchestration_Runner_WebApi>("OrchestrationRunnerService")
    .WithHttpsHealthCheck("/health");

var runnerPrototype = builder.AddProject<AiPipeline_TireOcr_RunnerPrototype>("RunnerPrototype")
    .WithHttpsHealthCheck("/health")
    .WithReference(preprocessingService)
    .WaitFor(preprocessingService)
    .WithReference(ocrService)
    .WaitFor(ocrService)
    .WithReference(postprocessingService)
    .WaitFor(postprocessingService);

builder.Build().Run();
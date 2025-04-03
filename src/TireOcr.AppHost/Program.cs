using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var preprocessingService = builder.AddProject<TireOcr_Preprocessing_WebApi>("PreprocessingService")
    .WithHttpsHealthCheck("/health");

var ocrService = builder.AddProject<TireOcr_Ocr_WebApi>("OcrService")
    .WithHttpsHealthCheck("/health");

var runnerPrototype = builder.AddProject<TireOcr_RunnerPrototype>("RunnerPrototype")
    .WithHttpsHealthCheck("/health")
    .WithReference(preprocessingService)
    .WaitFor(preprocessingService)
    .WithReference(ocrService)
    .WaitFor(ocrService);

builder.Build().Run();
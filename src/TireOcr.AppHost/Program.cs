using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var preprocessingService = builder.AddProject<TireOcr_Preprocessing_WebApi>("PreprocessingService")
    .WithHttpsHealthCheck("/health");

var ocrService = builder.AddProject<TireOcr_Ocr_WebApi>("OcrService")
    .WithHttpsHealthCheck("/health");

builder.Build().Run();
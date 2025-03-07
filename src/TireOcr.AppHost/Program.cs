using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var preprocessingService = builder.AddProject<TireOcr_Preprocessing_WebApi>("PreprocessingService")
    .WithHttpsHealthCheck("/health");

builder.Build().Run();
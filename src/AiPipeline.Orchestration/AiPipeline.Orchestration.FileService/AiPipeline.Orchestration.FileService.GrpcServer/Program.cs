using AiPipeline.Orchestration.FileService.Application;
using AiPipeline.Orchestration.FileService.GrpcServer;
using AiPipeline.Orchestration.FileService.GrpcServer.Extensions;
using AiPipeline.Orchestration.FileService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

app.AddGrpcServices();

app.Run();
using AiPipeline.TireOcr.TasyDbMatcher.Application;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure;
using AiPipeline.TireOcr.TasyDbMatcher.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPresentation(builder.Host);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
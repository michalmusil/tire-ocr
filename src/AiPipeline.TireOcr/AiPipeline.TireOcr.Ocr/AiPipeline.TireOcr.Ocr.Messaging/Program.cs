using AiPipeline.TireOcr.Ocr.Messaging;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    // .AddApplication()
    // .AddInfrastructure()
    .AddPresentation();

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();
app.Run();
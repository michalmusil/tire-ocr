using AiPipeline.TireOcr.Ocr.Messaging;
using TireOcr.Ocr.Application;
using TireOcr.Ocr.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPresentation(builder.Host, builder.Configuration);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
using AiPipeline.TireOcr.Ocr.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // .AddApplication()
    // .AddInfrastructure()
    .AddPresentation(builder.Host);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
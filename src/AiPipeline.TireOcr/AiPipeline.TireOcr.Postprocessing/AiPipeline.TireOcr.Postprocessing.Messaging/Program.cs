using AiPipeline.TireOcr.Postprocessing.Messaging;
using TireOcr.Postprocessing.Application;
using TireOcr.Postprocessing.Infrastructure;

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
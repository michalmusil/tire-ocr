using AiPipeline.TireOcr.TasyDbMatcher.Application;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure;
using AiPipeline.TireOcr.TasyDbMatcher.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
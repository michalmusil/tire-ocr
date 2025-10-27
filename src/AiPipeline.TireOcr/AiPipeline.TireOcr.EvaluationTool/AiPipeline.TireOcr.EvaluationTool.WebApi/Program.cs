using AiPipeline.TireOcr.EvaluationTool.Application;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure;
using AiPipeline.TireOcr.EvaluationTool.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
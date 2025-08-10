using AiPipeline.Orchestration.Runner.Application;
using AiPipeline.Orchestration.Runner.Infrastructure;
using AiPipeline.Orchestration.Runner.WebApi;
using AiPipeline.Orchestration.Runner.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration, builder.Host);

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();
app.AddSwagger();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
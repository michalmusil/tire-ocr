using AiPipeline.Orchestration.Runner.Application;
using AiPipeline.Orchestration.Runner.Infrastructure;
using AiPipeline.Orchestration.Runner.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Host);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
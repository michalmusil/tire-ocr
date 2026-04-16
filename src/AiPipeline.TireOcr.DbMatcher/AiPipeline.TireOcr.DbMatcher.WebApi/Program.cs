using AiPipeline.TireOcr.DbMatcher.Application;
using AiPipeline.TireOcr.DbMatcher.Infrastructure;
using AiPipeline.TireOcr.DbMatcher.WebApi;

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
using AiPipeline.Orchestration.Runner.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    // .AddApplication()
    // .AddInfrastructure()
    .AddPresentation(builder.Host);

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
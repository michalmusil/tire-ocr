using TireOcr.RunnerPrototype;
using TireOcr.RunnerPrototype.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation();

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();
app.AddSwagger();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
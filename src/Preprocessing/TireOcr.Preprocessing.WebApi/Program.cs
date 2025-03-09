using TireOcr.Preprocessing.Application;
using TireOcr.Preprocessing.Infrastructure;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;
using TireOcr.Preprocessing.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPresentation();

builder.AddServiceDefaults();

// Loading ML models into local storage
await using (var provider = builder.Services.BuildServiceProvider())
{
    var modelResolver = provider.GetRequiredService<IMlModelResolver>();
    await modelResolver.EnsureAllModelsLoadedAsync();
}


var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
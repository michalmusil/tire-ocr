using AiPipeline.TireOcr.Preprocessing.Messaging;
using TireOcr.Preprocessing.Application;
using TireOcr.Preprocessing.Infrastructure;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure()
    .AddPresentation(builder.Host, builder.Configuration);

builder.AddServiceDefaults();

await using (var provider = builder.Services.BuildServiceProvider())
{
    var modelResolver = provider.GetRequiredService<IMlModelResolverService>();
    await modelResolver.EnsureAllModelsLoadedAsync();
}

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
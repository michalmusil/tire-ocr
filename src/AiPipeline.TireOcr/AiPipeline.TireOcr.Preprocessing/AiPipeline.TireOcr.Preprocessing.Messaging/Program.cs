using AiPipeline.Orchestration.Shared.NodeSdk;
using TireOcr.Preprocessing.Application;
using TireOcr.Preprocessing.Infrastructure;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;


var app = await AiPipelineSharedNodeSdk
    .CreateNodeApplication(
        nodeId: "tire-ocr-preprocessing",
        provideRabbitMqConnectionString: builder =>
        {
            return builder.Configuration.GetConnectionString("rabbitmq") ??
                   throw new InvalidOperationException("RabbitMqConnectionString not present in configuration");
        },
        provideGrpcServerUri: _ => new Uri("http://FileService"),
        configureBuilder: async builder =>
        {
            builder.Services
                .AddApplication()
                .AddInfrastructure();
            
            builder.AddServiceDefaults();
            
            await using (var provider = builder.Services.BuildServiceProvider())
            {
                var modelResolver = provider.GetRequiredService<IMlModelResolverService>();
                await modelResolver.EnsureAllModelsLoadedAsync();
            }

        },
        assemblies: typeof(Program).Assembly
    );

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
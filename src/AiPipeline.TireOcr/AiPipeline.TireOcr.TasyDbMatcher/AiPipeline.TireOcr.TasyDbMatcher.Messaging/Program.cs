using AiPipeline.Orchestration.Shared.NodeSdk;
using AiPipeline.TireOcr.TasyDbMatcher.Application;
using AiPipeline.TireOcr.TasyDbMatcher.Infrastructure;

var app = await AiPipelineSharedNodeSdk
    .CreateNodeApplication(
        nodeId: "tire-ocr-tasy-db-matcher",
        provideRabbitMqConnectionString: builder =>
        {
            return builder.Configuration.GetConnectionString("rabbitmq") ??
                   throw new InvalidOperationException("RabbitMqConnectionString not present in configuration");
        },
        configureBuilder: builder =>
        {
            builder.Services
                .AddApplication()
                .AddInfrastructure(builder.Configuration);

            builder.AddServiceDefaults();
            return Task.CompletedTask;
        },
        assemblies: typeof(Program).Assembly
    );

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
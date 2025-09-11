using AiPipeline.Orchestration.Shared.NodeSdk;
using TireOcr.Ocr.Application;
using TireOcr.Ocr.Infrastructure;

var app = await AiPipelineSharedNodeSdk
    .CreateNodeApplication(
        nodeId: "tire-ocr-ocr",
        provideRabbitMqConnectionString: builder =>
        {
            return builder.Configuration.GetConnectionString("rabbitmq") ??
                   throw new InvalidOperationException("RabbitMqConnectionString not present in configuration");
        },
        provideGrpcServerUri: _ => new Uri("http://FileService"),
        configureBuilder: builder =>
        {
            builder.Services
                .AddApplication()
                .AddInfrastructure();

            builder.AddServiceDefaults();
            return Task.CompletedTask;
        },
        assemblies: typeof(Program).Assembly
    );

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();
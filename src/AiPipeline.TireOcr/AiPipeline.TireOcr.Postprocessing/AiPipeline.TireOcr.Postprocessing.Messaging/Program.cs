using AiPipeline.Orchestration.Shared.NodeSdk;

using TireOcr.Postprocessing.Application;
using TireOcr.Postprocessing.Infrastructure;

var app =  await AiPipelineSharedNodeSdk
    .CreateNodeApplication(
        nodeId: "tire-ocr-postprocessing",
        provideRabbitMqConnectionString: builder =>
        {
            return builder.Configuration.GetConnectionString("rabbitmq") ??
                   throw new InvalidOperationException("RabbitMqConnectionString not present in configuration");
        },
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
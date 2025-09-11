using System.Reflection;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.NodeSdk.Procedures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.NodeSdk.Producers;

public class NodeAdvertisementProducerService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly int _periodSeconds;

    private readonly NodeAdvertised _advertisement;

    public NodeAdvertisementProducerService(IServiceProvider serviceProvider, string nodeId,
        int periodSeconds = 30, params Assembly[] assemblies)
    {
        _serviceProvider = serviceProvider;
        _periodSeconds = periodSeconds;

        _advertisement = new NodeAdvertised
        {
            NodeId = nodeId,
            Procedures = GetProcedureDescriptorsFromAssemblies(assemblies)
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        while (!cancellationToken.IsCancellationRequested)
        {
            await bus.PublishAsync(_advertisement);

            await Task.Delay(_periodSeconds * 1000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private List<ProcedureDescriptor> GetProcedureDescriptorsFromAssemblies(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            return [];

        var procedureTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(type => typeof(IProcedure).IsAssignableFrom(type) &&
                           !type.IsInterface &&
                           !type.IsAbstract &&
                           !type.ContainsGenericParameters);

        var descriptors = new List<ProcedureDescriptor>();

        foreach (var procedureType in procedureTypes)
        {
            try
            {
                var procedureConstructor = procedureType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(c => c.GetParameters().Length)
                    .FirstOrDefault();
                if (procedureConstructor == null)
                    continue;
                
                var argsLength = procedureConstructor.GetParameters().Length;
                var args = new object[argsLength];
                
                var procedure = Activator.CreateInstance(procedureType, args: args) as IProcedure;

                if (procedure is null)
                    continue;

                var desciptor = new ProcedureDescriptor
                {
                    Id = procedure.Id,
                    SchemaVersion = procedure.SchemaVersion,
                    Input = procedure.InputSchema,
                    Output = procedure.OutputSchema
                };
                descriptors.Add(desciptor);
            }
            catch (Exception ex)
            {
            }
        }

        return descriptors;
    }
}
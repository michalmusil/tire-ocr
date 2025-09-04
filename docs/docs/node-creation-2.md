# Create Your Own Node

The AiPipeline platform is designed to be highly extensible, allowing developers to create and integrate custom nodes. While the platform is primarily built on .NET, making it easier to leverage the provided shared codebase, a node can theoretically be implemented in any programming language or framework.

This guide will focus on creating a .NET node named `word-processor`. This node will contain a single procedure called `repeat-string`, which accepts a string and an integer `N`, then outputs the string repeated `N` times. If `N` is not provided, the default repetition count is 2.

### 1. RabbitMQ Communication

RabbitMQ is the primary communication method for nodes within the AiPipeline platform. A node uses RabbitMQ for the following tasks:

1. Publishing Node Advertisements: Periodically advertising its availability and procedures to the Orchestration Runner.
2. Receiving Pipeline Commands: Listening on its dedicated queue for requests to process a pipeline step.
3. Sending Back Results: Notifying the Orchestration Runner of a step's successful completion or failure.
4. Forwarding Pipeline Commands: Sending the pipeline to the next node if the process continues.

#### Connecting to RabbitMQ

Your node must receive a connection string for the RabbitMQ broker. This is typically configured at the deployment level using an environment variable. For .NET nodes, the connection string is read from the configuration key `ConnectionStrings__rabbitmq`. The node must establish a connection to this broker on startup.

#### Setting Up Node Advertisements

Each node must periodically send an advertisement message to the Orchestration Runner to announce its availability and available procedures. For this purpose, a predefined exchange named `node-advertisements` is used.
To be compatible, your node should send an advertisement message to this exchange, for example, once every minute. The message must be a JSON object with the following structure:

```json
{
  "nodeId": "word-processor",
  "procedures": [
    {
      "id": "my-node-procedure-1",
      "schemaVersion": 1,
      "input": {
        "type": "ApObject",
        "properties": {
          "inputString": {
            "type": "ApString",
            "value": ""
          },
          "repetitionCount": {
            "type": "ApInt",
            "value": ""
          }
        },
        "nonRequiredProperties": ["repetitionCount"]
      },
      "output": {
        "type": "ApObject",
        "properties": {
          "repeatedString": {
            "type": "ApString",
            "value": ""
          }
        },
        "nonRequiredProperties": []
      }
    }
  ]
}
```

The `nodeId` at the root of the message is a unique identifier for your node. You can choose any arbitrary string, but it must be unique among all other nodes on the platform. The procedures array lists all procedures offered by your node, each with a unique `id`, a `schemaVersion` (set to `1` for now), and `input` and `output` schemas in the ApScheme format.

#### Creating a Custom Queue and Binding to a Topic

To receive pipeline commands, your node must declare a dedicated queue on the RabbitMQ broker. This queue **must be named the same as your node's ID**.
The Orchestration Runner uses a topic exchange named `run-pipeline` to send messages to individual nodes. To ensure your node's queue receives these messages, you must bind it to the topic `run-pipeline.<your-node-id>`. For our example, the topic would be `run-pipeline.word-processor`.

### 2. Implementing Your Procedures

After the initial RabbitMQ configuration, you can implement the actual logic for your node's procedures. Each procedure must have a unique ID within the node and defined ApScheme interfaces for its input and output. These parameters must be included in the periodic node advertisement.

#### Consuming and Routing Messages

Your node should be actively listening for messages on its dedicated queue. Upon receiving a message, it will have the following format:

```json
{
  "pipelineId": "da44763b-cb66-4e0b-a2c9-13c94541a4e8",
  "userId": "9b8ee2bc-9ecb-4556-89e0-4642f0a0cab4",
  "currentStep": {
    "nodeId": "word-processor",
    "procedureId": "repeat-string",
    "orderInPipeline": 3
  },
  "currentStepInput": {
    "type": "ApObject",
    "properties": {
      "inputString": {
        "type": "ApString",
        "value": "This is the input of the procedure"
      },
      "repetitionCount": {
        "type": "ApInt",
        "value": "3"
      }
    },
    "nonRequiredProperties": ["repetitionCount"]
  },
  "nextSteps": [
    {
      "nodeId": "next-node-id-1",
      "procedureId": "next-node-requested-procedure-id",
      "orderInPipeline": 4,
      "outputValueSelector": "0.innerParamXyz"
    },
    {
      "nodeId": "next-node-id-2",
      "procedureId": "next-node-requested-procedure-id-2",
      "orderInPipeline": 5
    }
  ],
  "fileReferences": [
    {
      "id": "92d95e90-5c3f-45aa-ab04-e1533e5153ee",
      "storageProvider": "minio",
      "path": "short-term-files/92d95e90-5c3f-45aa-ab04-e1533e5153ee.png",
      "contentType": "image/png"
    }
  ]
}
```

When a message arrives, your node must:

1. Identify the procedure to execute using the `procedureId` from the `currentStep` object.
2. Route the message to the corresponding procedure.
3. Provide the `currentStepInput` as the procedure's input data.

#### Executing the Procedure

During execution, your procedure should first parse the ApScheme input into usable data, then perform its core logic. Upon successful completion, you must:

1.  Construct an ApScheme result that matches your procedure's output schema.
2.  Publish a message to the `completed-pipeline-steps` exchange to inform the Orchestration Runner of the successful completion. The message should contain:
    - `pipelineId`, `nextSteps`, and `procedureIdentifier` from the original message.
    - `completedAt`, the exact time of completion in ISO 8601 format.
    - `result`, which is your procedure's ApScheme output. If an `outputValueSelector` was used in the original `currentStep`, apply the selector to your full result and return only the specified portion.
3.  If there are any `nextSteps` in the original message, you must publish a new message for the next node. This message is sent to the `run-pipeline` exchange with the topic `run-pipeline.<next-node-identifier>`. The new message must be a modified version of the original:
    - The `currentStep` is replaced with the first step from `nextSteps`.
    - `currentStepInput` is replaced with your procedure's ApScheme `output`.
    - The first step is removed from the `nextSteps` list.
    - Any new files produced by your procedure are added to the `fileReferences` list.

#### Handling Pipeline Failures

If your procedure fails, it should terminate the pipeline and notify the Orchestration Runner. Publish a message to the `failed-pipelines` exchange with the following format:

```json
{
  "pipelineId": "da44763b-cb66-4e0b-a2c9-13c94541a4e8",
  "procedureIdentifier": {
    "nodeId": "word-processor",
    "procedureId": "repeat-string",
    "orderInPipeline": 3
  },
  "failedAt": "2024-01-24T06:09:19.384957Z",
  "failureCode": 422,
  "failureReason": "The provided repetition count wasn't a positive number ('-20'), which is not allowed.",
  "exceptionMessage": "This is an optional string where you can provide exception details if your pipeline failed unexpectedly",
  "remainingNotCompletedSteps": [
    {
      "nodeId": "next-node-id-1",
      "procedureId": "next-node-requested-procedure-id",
      "orderInPipeline": 4,
      "outputValueSelector": "0.innerParamXyz"
    },
    {
      "nodeId": "next-node-id-2",
      "procedureId": "next-node-requested-procedure-id-2",
      "orderInPipeline": 5
    }
  ]
}
```

The `failureCode` should be a numeric value, semantically consistent with HTTP status codes. The `failureReason` provides a human-readable description of the error, and `exceptionMessage` is an optional field for technical details.

#### Handling Files

The platform includes a gRPC file service for managing files. The protocol buffer (.proto) file with the interface definition is located in the `AiPipeline.Orchestration.FileService.GrpcServer/Protos` folder.

- Downloading Files: If a procedure receives an ApFile in its input, it must find the corresponding file reference in the `fileReferences` list and use the gRPC file service to download the data.
- Uploading Files: If a procedure generates a new file, it should upload it to the gRPC file service. You must provide the file's name, data, content type, a new GUID for the file, the user's GUID (from the incoming message), and a `storageScope`. The storage scope determines how long the file will be kept and can be one of three values: `long-term-files`, `short-term-files`, or `temp-files`. After uploading, the new file reference must be added to the `fileReferences` list if the pipeline continues to a subsequent step.

## Example Node in .NET

Most of the logic described above is already prepared in the shared codebase for .NET. The following is a step-by-step guide on how to create the `word-processor` node using .NET.

### 1. Initialize Your Node Project

Create a new ASP.NET Core Web API project in the AiPipeline solution. Name it `AiPipeline.<optional-node-group-name>.WordProcessor`.

Add project references to the following shared projects:

- `AiPipeline.Orchestration.Shared.All`: Provides messaging constants, contracts, and ApScheme definitions.
- `AiPipeline.Orchestration.Shared.Nodes`: Contains utilities for nodes, such as message routing, the procedure interface, and file services.
- `AiPipeline.ServiceDefaults`: For debugging purposes with .NET Aspire.
- `AiPipeline.Shared`: Includes general utilities used throughout the codebase.

### 2. Declare Messaging Constants

To define your node's unique ID, add a new constant to the `MessagingConstants` static class located in `AiPipeline.Orchestration.Shared.All`.

```csharp
public static class MessagingConstants
{
    // ... other constants
    public static string WordProcessorId => "word-processor";
}
```

Next, ensure the `run-pipeline` exchange is configured for your new node's queue. In `WolverineExtensions.cs` within `AiPipeline.Orchestration.Shared.All`, add the following lines to the `DeclareExchanges` extension method:

```csharp
public static RabbitMqTransportExpression DeclareExchanges(this RabbitMqTransportExpression expression)
{
    return expression
        .DeclareExchange(MessagingConstants.RunPipelineExchangeName, exc =>
        {
          // ... other bindings
          exc.BindTopic($"{MessagingConstants.RunPipelineExchangeName}.{MessagingConstants.WordProcessorId}")
            .ToQueue(MessagingConstants.WordProcessorId);
        });
}
```

### 3. Set Up Program.cs

Configure the `Program.cs` file in your new node project to handle RabbitMQ communication and service registration:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configures connection to RabbitMq message broker via WolverineFx
builder.Host.UseWolverine(opt =>
        {
            opt.UseRabbitMqUsingNamedConnection("rabbitmq") // This connects your node to the RabbitMq broker. The connectionString should added to configuration as 'ConnectionStrings__rabbitmq'
                .DeclareExchanges() // Ensures all exchanges and queues are properly declared
                .AutoProvision(); // Ensures RabbitMq configuration is persisted on the broker

            opt.ConfigureMessagePublishing(); // Adds rules on how the messages should be published in the RabbitMq broker
            opt.ApplyCustomConfiguration(); // Adds custom configuration on message parsing (ApElements)

            opt.ListenToRabbitQueue(MessagingConstants.WordProcessorId); // Ensures your node receives messages for procedure execution

            // Adds a separate hosted service which will publish your node and procedure advertisements to the OrchestrationRunner
            opt.Services.AddHostedService(provider =>
                new NodeAdvertisementProducerService(
                    serviceProvider: provider,
                    nodeId: MessagingConstants.WordProcessorId,
                    assemblies: typeof(DependencyInjection).Assembly
                )
            );
        });

// Injects 'IFileReferenceDownloaderService' and 'IFileReferenceUploaderService' to work with files via gRPC File Service
builder.Services.AddFileManipulation(builder.Configuration);
// Injects procedure routing and registers all procedures (implementations of IProcedure) from your assemblies
builder.Services.AddProcedureRoutingFromAssemblies(typeof(Program).Assembly);

var app = builder.Build();
app.Run();
```

### 4. Implement Your Procedure

Create a new class named `RepeatStringProcedure` that implements the `IProcedure` interface.

```csharp
public class RepeatStringProcedure : IProcedure
{
    public string Id => "repeat-string";
    public int SchemaVersion => 1;

    public IApElement InputSchema => new ApObject(
        properties: new Dictionary<string, IApElement>()
        {
            { "inputString", ApString.Template() },
            { "repetitionCount", ApInt.Template() },
        },
        nonRequiredProperties: ["repetitionCount"]
    );

    public IApElement OutputSchema => new ApObject(
        properties: new Dictionary<string, IApElement>()
        {
            { "repeatedString", ApString.Template() }
        }
    );

    public async Task<DataResult<IApElement>> ExecuteAsync(RunPipelineStep step)
    {
        var inputValuesResult = ExtractInputValues(step.CurrentStepInput);
        if (inputValuesResult.IsFailure)
            return DataResult<IApElement>.Failure(inputValuesResult.Failures);

        var inputValues = inputValuesResult.Data!;
        var result = string.Concat(Enumerable.Repeat(inputValues.Item1, inputValues.Item2));

        return DataResult<IApElement>.Success(
            new ApObject(
                properties: new()
                {
                    { "repeatedString", new ApString(result) }
                }
            )
        );
    }

    private DataResult<(string, int)> ExtractInputValues(IApElement stepInput)
    {
        if (stepInput is ApObject input)
        {
            var hasInputString = input.TryGetValueCaseInsensitive("inputString", out var inputString);
            if (!hasInputString || inputString is not ApString)
                return DataResult<(string, int)>.Invalid(
                    $"Property 'inputString' is required and must be of type {nameof(ApString)}");

            input.TryGetValueCaseInsensitive("repetitionCount", out var repCnt);
            var repetitionCount = repCnt is ApInt repCntInt ? repCntInt.Value : 2;
            if (repetitionCount < 1)
                return DataResult<(string, int)>.Invalid(
                    $"The provided repetition count wasn't a positive number ('{repetitionCount}'), which is not allowed.");

            return DataResult<(string, int)>.Success((
                ((ApString)inputString).Value,
                repetitionCount
            ));
        }

        return DataResult<(string, int)>.Invalid("Input has an invalid scheme.");
    }
}
```

The `ExecuteAsync` method handles only the core logic. All messaging, failure/success handling, and routing to subsequent steps are managed automatically by the `ProcedureRouter`, which was registered in `Program.cs`.

### 5. Run Your Node

To run your node in a local debug environment with .NET Aspire, you must add a project reference to your node from the Aspire runner project (`AiPipeline.AppHost`). In the `Program.cs` file of `AiPipeline.Apphost`, add the following:

```csharp
var wordProcessorNode = builder.AddProject<AiPipeline_WordProcessor>("WordProcessorNode")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(fileService);
```

For production, add a Dockerfile to the root of your project and configure your deployment solution (e.g., Kubernetes, Docker Compose) to include your new service. Be sure to provide all necessary connection strings and secrets.

# Creating a Custom Node

The AiPipeline platform is designed to be highly extensible. You can add new functionalities by creating and integrating your own custom nodes. While the platform is primarily built in .NET, a node can be implemented in any language that can communicate with RabbitMQ and gRPC.

This guide uses a simple `word-processor` node as an example. Its single procedure, `repeat-string`, takes a string and a repetition count N as input, then outputs the string repeated N times. N is optional and it's default is 2.

---

### 1. Node Communication Essentials

Nodes communicate with the Orchestration Runner and other nodes via **RabbitMQ**, a message broker. Your node needs to handle four key messaging tasks:

1. **Advertising**: Periodically announce your node's capabilities to the Orchestration Runner.
2. **Listening**: Receive pipeline commands from the Orchestration Runner (or other nodes).
3. **Reporting**: Send the results (success or failure) of a procedure back to the runner.
4. **Forwarding**: Pass the pipeline command to the next node in the sequence.

#### RabbitMQ Setup

Your node needs a connection string to the RabbitMQ broker, typically provided via a `ConnectionStrings__rabbitmq` environment variable. On startup, it must establish a connection.

#### Node Advertisements

Your node must periodically send an advertisement message to the `node-advertisements` exchange. This message informs the Orchestration Runner about your node's availability and its procedures.

```json
{
  "nodeId": "word-processor",
  "procedures": [
    {
      "id": "repeat-string",
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

The `nodeId` must be a globally unique string. The procedures array lists all the functionalities your node provides, including their node-unique IDs and ApScheme input/output contracts.

#### Dedicated Queue

To receive work, your node needs its own queue. This queue **must have the same name as your `nodeId`**. You then need to bind this queue to the `run-pipeline` topic exchange using a topic pattern that matches your queue name: `run-pipeline.<your-node-id>`. This setup ensures that your node only receives messages intended for it.

### 2. Implementing the Procedure Logic

Once the messaging setup is complete, you can focus on the core logic of your procedures.

#### Handling Incoming Messages

When a message arrives on your queue, it will contain all the information needed to execute a step in a pipeline.

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

Your node must:

1. **Extract** the `procedureId` from the `currentStep` object.
2. **Route** the message to the corresponding procedure's execution logic.
3. **Provide** the `currentStepInput` as the procedure's input data.

#### Success: Completing a Step

If your procedure runs successfully, you must do two things:

1. **Report Success**: Publish a message to the `completed-pipeline-steps` exchange. This message should contain the `pipelineId`, the procedure's identifier, a timestamp, and your procedure's ApScheme output as the `result`. If the original message had an `outputValueSelector`, you must apply it to your output before sending.

```json
{
  "pipelineId": "da44763b-cb66-4e0b-a2c9-13c94541a4e8",
  "procedureIdentifier": {
    "nodeId": "word-processor",
    "procedureId": "repeat-string",
    "orderInPipeline": 3
  },
  "completedAt": "2024-01-24T06:09:19.384957Z",
  "result": {
    "type": "ApObject",
    "properties": {
      "repeatedString": {
        "type": "ApString",
        "value": "This is the input of the procedureThis is the input of the procedureThis is the input of the procedure"
      }
    }
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
  ]
}
```

2.**Forward Pipeline**: If the nextSteps array is not empty, create a new message for the next node. This message is identical to the one you received, but with the following changes:

- The `currentStep` is now the first step from the original `nextSteps`.
- The `currentStepInput` is now your procedure's output (or value selected by optional `outputValueSelector`).
- The first step is removed from the `nextSteps` array.
- Any new file references are added to the `fileReferences` list.

```json
{
  "pipelineId": "da44763b-cb66-4e0b-a2c9-13c94541a4e8",
  "userId": "9b8ee2bc-9ecb-4556-89e0-4642f0a0cab4",
  "currentStep": {
    "nodeId": "next-node-id-1",
    "procedureId": "next-node-requested-procedure-id",
    "orderInPipeline": 4,
    "outputValueSelector": "0.innerParamXyz"
  },
  "currentStepInput": {
    "type": "ApObject",
    "properties": {
      "repeatedString": {
        "type": "ApString",
        "value": "This is the input of the procedureThis is the input of the procedureThis is the input of the procedure"
      }
    }
  },
  "nextSteps": [
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

Publish this new message to the `run-pipeline` exchange with the topic `run-pipeline.<next-node-id>`.

#### Failure: Handling Errors

If your procedure fails, it must stop the pipeline and report the error. Publish a single message to the `failed-pipelines` exchange. This message should include:

- `pipelineId` and `procedureIdentifier` to identify the failure.
- `failedAt` timestamp.
- A `failureCode` (a numeric code semantically consistent with HTTP status codes, e.g., 422 for bad input).
- A descriptive `failureReason`.
- An optional `exceptionMessage` for technical details.
- The `remainingNotCompletedSteps` to show which parts of the pipeline were skipped.

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

#### Handling Files

The platform includes a dedicated **gRPC file service** for managing file data.

- **Download**: If an `ApFile` is in your input, find its ID in the `fileReferences` list. Use the gRPC service to download the file data for processing.
- **Upload**: If your procedure generates a new file, upload it to the gRPC service. You must provide the file's data, its `contentType`, a new GUID, the `userId` from the incoming message, and a `storageScope`. The storage scope determines how long the file will be kept and can be one of three values: `long-term-files`, `short-term-files`, or `temp-files`. The service will return a reference that must be added to the `fileReferences` list if the pipeline continues.

---

### Example Node in .NET

The shared .NET codebase handles most of the complex messaging and routing logic for you. Here’s a streamlined guide for implementing the `word-processor` example.

#### Project Setup

1. Create a new Web API project in the solution: `AiPipeline.<optional-node-group-name>.WordProcessor`.
2. Add references to the following shared projects: `AiPipeline.Orchestration.Shared.All`, `AiPipeline.Orchestration.Shared.Nodes`, `AiPipeline.ServiceDefaults` (only for local deployment), and `AiPipeline.Shared`.

#### Declare Messaging Constants

In the `MessagingConstants.cs` file (in `AiPipeline.Orchestration.Shared.All`), add your node's unique ID.

```csharp
public static class MessagingConstants
{
    // ... other constants
    public static string WordProcessorId => "word-processor";
}
```

Next, in `WolverineExtensions.cs`, bind your node's queue to the `run-pipeline` exchange.

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

#### Program.cs Configuration

Configure `Program.cs` to set up Wolverine, register your procedures, and enable file services. The shared utilities handle all the background work.

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

#### Implement Your Procedure

Create a `RepeatStringProcedure` class that implements the `IProcedure` interface. You only need to define its ID, schemas, and the `ExecuteAsync` method.

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

The `ProcedureRouter` takes care of all the message-passing and error-handling boilerplate. Your `ExecuteAsync` method only needs to focus on the business logic and returning a `DataResult`.

#### Running Your Node

To run your node in a local Aspire debug environment, add a project reference to it in the `AiPipeline.Apphost` project.

```csharp
// ... other projects
var wordProcessorNode = builder.AddProject<AiPipeline_WordProcessor>("WordProcessorNode")
    .WithReference(rabbitMq) // rabbitMq already prepared in Program.cs
    .WaitFor(rabbitMq)
    .WithReference(fileService) // fileService already prepared in Program.cs;
```

For production, containerize your project with a Dockerfile and add it to your deployment configuration (e.g., Kubernetes, Docker Compose), ensuring you provide all necessary secrets and connection strings.

#### A Note on DataResult and Result

The `DataResult<T>` and `Result` classes are not a mandatory part of building an AiPipeline node, but they are an integral utility within the shared codebase. They are a C# implementation of a union type, designed to explicitly handle success and failure states in a clean, composable way.

Instead of throwing exceptions or returning `null` to indicate a failure, these classes allow you to return a single object that clearly contains either the expected data on success or a collection of `Failure` objects on failure.

This pattern is essential when implementing the `IProcedure` interface, as the `ExecuteAsync` method requires a return type of `Task<DataResult<IApElement>>`. The `ProcedureRouter` relies on this explicit result to automatically handle the next steps in the pipeline:

- If `IsSuccess` is `true`, the `ProcedureRouter` takes the `Data` property and passes it to the next node.
- If `IsSuccess` is `false`, the router uses the `Failures` property to construct the failure message and publish it to the `failed-pipelines` queue.

The static factory methods on the `Result` and `DataResult<T>` classes provide a simple way to create these objects for common scenarios. They align with standard HTTP status codes, such as `Invalid(422)` for bad input or `NotFound(404)` for missing resources, making the failure reasons clear and consistent across the platform.

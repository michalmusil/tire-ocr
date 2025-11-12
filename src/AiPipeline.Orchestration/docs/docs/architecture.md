# AiPipeline Platform Architecture Overview

The AiPipeline platform provides a scalable solution for complex data-processing pipelines through node-to-node communication. The system is built with a decoupled architecture, allowing each node to be deployed independently in either cloud or on-premise environments. The platform is composed of three core components: the Orchestration Runner, the File Service, and the individual Nodes.

All core components are implemented in .NET 8 (with an exception of Nodes, which may be extended and implemented using any platoform) and follow Clean Architecture, CQRS, and microservice principles. This layered approach ensures a clear separation of concerns between the domain, application, infrastructure, and presentation layers.

## Communication Protocols

To ensure smooth and efficient operation, the AiPipeline platform utilizes different communication technologies tailored to each specific task.

- **User-to-System**: The Orchestration Runner exposes a RESTful API, serving as the single public entry point for users to interact with the platform.
- **Inter-component Communication (Runner-to-Node & Node-to-Node)**: RabbitMQ is used as a message broker to facilitate the distribution and orchestration of pipelines. The runner initiates a pipeline by sending a message to the first node. Each subsequent node processes the message, sends the result to the next node in the pipeline, and sends a completion notification back to the runner.
- **File Handling (Node-to-FileService & Runner-to-FileService)**: For server-to-server communication with the File Service, gRPC is the protocol of choice. Its minimal overhead and high efficiency make it ideal for the transfer of large file data.

## Core Components

### 1. Orchestration Runner (AiPipeline.Orchestration.Runner)

The Orchestration Runner is the central nervous system of the platform. It's a web service that provides the only public-facing API, allowing users to:

- Initiate and run pipelines.
- Analyze pipeline results.
- Manage user accounts and pipeline files.

Its primary responsibilities include pipeline creation, validation, and mapping the correct sequence of nodes for a given process. All other platform components, including the nodes and the File Service, are hosted privately and are not publicly exposed.

### 2. Nodes (AiPipeline.YourNodeName)

A node is an independent service integrated into the AiPipeline platform by implementing a specific set of interfaces (described in [Create your own node](node-creation.md)). Each node can have multiple procedures, each with a defined input and output scheme. Users can chain these procedures together using the Orchestration Runner to create complex, long-running pipelines.

For scalability, the same type of node (same identifier, procedures and interface) can be deployed as multiple duplicates. The Orchestration Runner automatically distributes the workload of running pipelines evenly among these duplicates, which is crucial for handling parallel processing or computationally intensive tasks.

### 3. File Service (AiPipeline.Orchestration.FileService)

The File Service provides a unified API for nodes to manage files (e.g., images, documents, videos) used within pipelines. It is a private service, accessible only by other components within the platform deployment. This ensures secure handling of sensitive data.

**Important Note**: Only files residing within the File Service can be processed by pipelines. Users can manage their files through the Orchestration Runner's public API.

The File Service itself is a standalone .NET service that uses a PostgreSQL database to track file records and an S3-compatible storage solution (MinIO) to store the actual file data. Due to its server-to-server communication pattern and focus on large data transfers, it exclusively uses a gRPC API for efficiency.

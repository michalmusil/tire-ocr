# Orchestration Runner

The Orchestration Runner serves as the central control for the AiPipeline platform. It manages several core functionalities and provides a public RESTful API with dedicated endpoint groups for user interaction.

## 1. Authentication and Authorization (Auth)

Access to the AiPipeline platform is restricted to authenticated users and services. The Authentication API provides a secure way to manage access.

- `POST /Register`: Creates a new user account with provided credentials.
- `POST /Login`: Authenticates a user and returns a pair of access and refresh tokens.
- `POST /Refresh`: Exchanges an existing token pair for a new one, extending the authenticated session.
- `POST /ApiKey`: Generates a new, uniquely named API key for current user. This key can be used by other services to access the platform. The API key is only returned once and cannot be retrieved later.
- `DELETE /ApiKey/{name}`: Deletes the specified API key.

### Authentication Methods

- **For Users**: Use the access token from the /Login endpoint. Include it in the Authorization HTTP header with the Bearer scheme => `Authorization: Bearer <your_access_token>`.
- **For Services**: Use the API key from the /ApiKey endpoint. Include it directly in the Authorization HTTP header => `Authorization: <your_api_key>`.

### Authorization Rules

Authenticated users have full access to all endpoints. However, authorization is scoped to the user, meaning they can only view and manage content they have created, including files, pipeline results, and personal data. This restriction does not apply to the Node Discovery endpoints, which are public for all authenticated users.

## 2. File Management (Files)

The Orchestration Runner provides an API for users to interact with the central File Service. Internally, the runner communicates with the File Service using its gRPC SDK.

- `GET /Files`: Lists all files owned by the user.
- `GET /Files/{id}`: Retrieves the structured metadata for a specific file.
- `GET /Files/{id}/Download`: Downloads the raw data of a file.
- `POST /Files/Upload`: Uploads a new file, creating a new file record and storing the data in the File Service.
- `DELETE /Files/{id}`: Deletes a file record and its associated data.

## 3. Node Discovery (Nodes)

This functionality provides a simple way to discover and inspect all available nodes on the platform. The Orchestration Runner keeps up-to-date records of all available nodes in it's database. It gathers this data by listening on a RabbitMQ queue, which is used by the nodes to advertise themselves, their procedures and their i/o schemes.

- `GET /Nodes`: Lists all available nodes the Orchestration Runner sees. These records contain the nodes' procedures and the procedures' i/o schemes

## 4. Running pipelines (Pipelines)

This is the core functionality of the Orchestration Runner. Endpoints in this group allow the user to construct an arbitrary pipeline from gathered information about available nodes. Generally user outlines the flow of the pipeline using node-procedure identifier list and provides the initial input (compatible with the first procedure in the sequence). More info on how to run pipelines available on [Running Pipelines](running-pipelines.md).

- `POST /Pipelines`: Runs a pipeline on the background, not awaiting the result.
- `POST /Pipelines/Awaited`: Runs a pipeline in the same manner as in the standard /Pipelines endpoint, but the Runner only responds after the pipeline has completed (or user-provided timeout runs out). Returns a pipeline result directly back to the user
- `POST /Pipelines/Batch`: Runs the same pipeline on multiple inputs in parallel. Number of pipelines is derrived from the number of provided inputs. Completion of pipelines is not awaited.

## 5. Pipeline Result Analysis (PipelineResults)

This functionality is focused simply on listing and display of results of pipelines, where user can find step-by-step process of the finished pipeline, seeing each partial result or reason of failure in case pipeline doesn't finish successfully.

- `GET /PipelineResults`: Lists results of all current user pipelines
- `GET /PipelineResults/{id}`: Gets a result of current user pipeline with the specified pipeline id.
- `GET /PipelineResults/Batch/{id}`: Gets a sequence of pipeline results for pipelines ran from a single batch (using the Pipelines/Batch endpoint).

## 6. User Management (Users)

Users can manage their accounts through this set of endpoints.

- `GET /Users/{id}`: Retrieves public details of a user.
- `PUT /Users/{id}`: Updates the authenticated user's account details.
- `DELETE /Users/{id}`: Deletes the authenticated user's account and all associated data

<!-- # Orchestration runner

As mentioned on previous pages, Orchestration Runner is the real brain of AiPipeline platform. It manages several key functionality components of the system while providing a public API with endpoint groups for each of these components, so that users may use them. These functionalities are:

## 1. Authentication and Authorization (Auth)

Only an authenticated user/service may use the AiPipeline platform. For this purpose, Orchestration Runner API provides a group with following endpoints:

- **POST:Register** - User enter their new credentials to register to the platform.
- **POST:Login** - User enters their credentials and receive an Access/Refresh token pair.
- **POST:Refresh** - By providing an access/refresh token pair back to the server using this endpoint, user will get a fresh pair of these tokens back, thus prolonging their authenticated period.
- **POST:ApiKey** - User creates and gets a new ApiKey with a unique non-empty name with optional expiration. This ApiKey may then be used by other services to access the AiPipeline platform. The ApiKey can't be accessed outside of response to this request.
- **DELETE:ApiKey/{name}**: User deletes their api key with the respective name.

### How to authenticate

- **Users**: Insert the access token received from Login endoint into the Authorization HTTP Header preceded by 'Bearer '. Example: 'Bearer eyJhbGciO.....s'.
- **Services**: Insert the ApiKey received from ApiKey endpoint directly into the Authorization HTTP header. Example: '=ImF1ZCI6ImFpLXBpcGVsaW5lLmNvbSJ..'.

### How is authorization configured

Every authenticated user may use all the provided endpoints. With exception of Nodes endpoints, users are only authorized to see and manage content generated by them. That includes uploaded files, pipeline results, user-data and more.

## 2. File Management (Files)

Orchestration Runner provides an API to communicate with the central file service. Through this endpoint group, users may upload new files, or display, download and delete files created by them or pipelines initiated by them. Internally Runner just uses FileService's gRPC sdk for communication. Available endpoints are:

- **GET:Files** - User lists all files available to them
- **GET:Files/{id}** - User accesses structured record of their file
- **GET:Files/{id}/Download** - User downloads their file's data
- **POST:Files/Upload** - User uploads a new file, creating a record and storing the file data
- **DELETE:Files/{id}** - User deletes their file record together with the file data

## 3. Node Discovery (Nodes)

This functionality provides just a single endpoint for listing all available nodes on the platform. The Orchestration Runner keeps up-to-date records of all available nodes in it's database. It gathers this data by listening on a RabbitMQ queue, which is used by the nodes to advertise themselves, their procedures and their i/o shemes. Available endpoints:

- **GET:Nodes** - User lists all available nodes the Orchestration Runner sees. These records contain the nodes' procedures and the procedure's i/o schemes

## 4. Running pipelines (Pipelines)

This is the core functionality of the Orchestration Runner. Endpoints in this group allow the user to construct an arbitrary pipeline from gathered information about available nodes. Generally user outlines the flow of the pipeline using node-procedure identifier list and provides the initial input (compatible with the first procedure in the sequence). More info on how to run pipelines available on [Running Pipelines](running-pipelines.md) Available endpoints:

- **POST:Pipelines** - User runs a pipeline on the background, not awaiting the result.
- **POST:Pipelines/Awaited** - User runs a pipeline in the same manner as in the standard /Pipelines endpoint, but the Runner only responds after the pipeline has completed (or user-provided timeout runs out). Returns a pipeline result directly back to the user
- **POST:Pipelines/Batch** - User runs the same pipeline on multiple inputs in parallel. Number of pipelines is derrived from the number of provided inputs. Completion of pipelines is not awaited.

## 5. Pipeline Result Analysis (PipelineResults)

This functionality is focused simply on listing and display of results of pipelines, where user can find step-by-step process of the finished pipeline, seeing each partial result or reason of failure in case pipeline doesn't finish successfully. Available endpoints:

- **GET:PipelineResults** - User lists results of all their pipelines
- **GET:PipelineResults/{id}** - User gets a result of their pipeline with the specified pipeline id.
- **GET:PipelineResults/Batch/{id}** - User gets a sequence of pipeline results from pipelines ran from a single batch (using the Pipelines/Batch endpoint).

## 6. User management (Users)

Orchestration Runner also provides a simple functionality where users may manage their accounts. Available endpoints:

- **GET:Users/{id}** - User displays public details of user with given id.
- **PUT:Users/{id}** - User updates their own account detail.
- **DELETE:Users/{id}** - User deletes their own account and all data linked to it. -->

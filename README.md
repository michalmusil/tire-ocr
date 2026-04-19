# Tire Code Text Recognition Evaluation Framework

This archive represents the implementation of **evaluation framework**, a core part of the thesis: "Evaluation and Application of Contemporary Text Recognition Solutions for Tire Parameter Extraction"

## Codebase Structure

The presented codebase follows a systematic structure. Following sections describe the fundamentals.

#### deploy folder

The root `deploy` folder contains necessary scripts and configurations needed to deploy the entire containerized evaluation framework via `docker compose`.

Subfolder `opencvsharp` contains shell scripts and a Dockerfile needed for building a docker image used for preparing native dependencies of `OpenCVSharp` library (a .NET wrapper for OpenCV). The pre-built resulting image is available on docker hub registry (`michalmusil/dotnet8-noble-opencv`) and used during the the build of the .NET Preprocessing service's image build.

Subfolder `evaluation-framework` contains shell scripts, which when ran from this archive's root ensure building (`build-images.sh`) and publishing (`publish-images.sh`) all the docker images needed for deploying the containerized evaluation framework. It also contains `docker-compose.yaml`, which serves as a manifest for deploying the framework via `docker compose`.

#### src folder

The root `src` folder contains all the source code of the evaluation framework. Code of individual services of the framework, each being located in a dedicated subfolder is located here, grouped by service name.

Architecture and structure of all services is based on the [Clean/Onion architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). In summary, this means the codebase of each service is structured into several projects/root folders - **Domain** (where applicable), **Application**, **Infrastructure** and **Presentation** (name depending on the technology).

---

## Thesis Terminology Differences

This codebase is a result of over a year of work (including a lot of experimentation) which culminated in the evaluation framework presented in the thesis. As a result, there are several discrepancies in terminology between the thesis and this codebase, which are as follows:

- **Usage of the OCR abbreviation**: The term `OCR` is historically and contemporarily more ubiquitous than `Text Recognition`, which was, however, ultimately chosen as more accurate for the thesis text. As a result, both the `Recognition` module and the recognition process itself described in the thesis is referred as `OCR` in this codebase.
- **Project prefix `AiPipeline.TireOcr`**: In the early stages, the codebase design differed slightly compared the the final desin presented in the thesis. As a result, now redundant `AiPipeline` root prefix is used for all service namespaces.
- **Preprocessing Variants**: While the thesis text focused on the most promising preprocessing variants, which were ultimately evaluated, there were substantionaly more preprocessing variants experimentally investigated during the implementation phase. Preprocessing variants are differentiated by an enum located in `src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.Domain/StepTypes/PreprocessingType.cs`. The following mapping describes which enum values correspond to preprocessing variant designations in the thesis text:
  - **Variant1**: `ExtractAndComposeSlices`
  - **Variant2**: `ExtractTextsIntoMosaic`
  - **Variant3**: `ExtractAbsoluteRoi`
- **Text Recognition Solutions**: Similarily to preprocessing variants, there were more experimentally investigated TR solutions then the final evaluated makeup described in the thesis. Within the evaluation framework, TR solutions are differentiated using a dedicated enum in `src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.Domain/StepTypes/OcrType.cs`. Their mapping to the identifiers used in the thesis text is as follows:
  - **Gemini 2.5 Flash**: `GoogleGemini`
  - **GPT-4.1**: `OpenAiGpt`
  - **Qwen3-VL 8B Instruct**: `QwenVl`
  - **HunyuanOcr**: `HunyuanOcr`
  - **Google Cloud Vision**: `GoogleCloudVision`
  - **Azure Ai Vision**: `AzureAiVision`
  - **PP-OCRv5**: `PaddleOcr`
  - **EasyOcr**: `EasyOCR`

---

## Services overview

- **AiPipeline.TireOcr.Preprocessing**: a .NET service providing the functionality for preprocessing variants 1 and 3 (and other experimental preprocessing variants not included in the evaluation).
- **AiPipeline.TireOcr.PythonPreprocessing**: a Python service providing the functionality for preprocessing variant 2 (and other experimental preprocessing variants not included in the evaluation).
- **AiPipeline.TireOcr.Ocr**: a .NET service facilitating the functionality of remote-accessed TR solutions (like Gemini, GPT, Qwen, HunyuanOCR, Google Ai Vision and Azure Ai Vision).
- **AiPipeline.TireOcr.PythonOcr**: a Python service facilitating the functionality locally hosted TR solutions (PP-OCRv5 and EasyOCR).
- **AiPipeline.TireOcr.Postprocessing**: a .NET service implementing the domain-specific postprocessing procedure.
- **AiPipeline.TireOcr.DbMatcher**: a .NET service implementing the tire code matching procedure included in the final production service prototype. This service isn't relevant for the evaluation process and was included as a part of this framework only in preparation for the production prototype implementation.
- **AiPipeline.TireOcr.EvaluationTool**: a .NET service implementing the functionality of the **Evaluation Service** described in the thesis evaluation framework design. This service acts as the core orchestrator of the evaluation framework, invoking pipelines and processing/storing the results.
- **AiPipeline.TireOcr.EvaluationTool.Ui**: a Vite React application serving as a front-end for the evaluation service. This SPA is written in typescript and provides a simplified interface for invoking pipelines/pipeline batches and analyzing/exporting the results.
- **AiPipeline.Shared**: a .NET class library project containing the shared core components used across all implemented .NET services.
- **AiPipeline.ServiceDefaults**: a .NET class library project providing fundamental configuration defaults used acrosss all implemented .NET services.
- **AiPipeline.AppHost**: a [.NET Aspire](https://aspire.dev/) project enabling seamless orchestration and running of the evaluation framework for development environments.

---

## How to Run

There are two main ways of running the evaluation framework described below.

#### Docker Compose (for server deployment)

All constituent services of the evaluation framework were containerized for running in the `docker` environment. All services contain a `Dockerfile` for building the image in their root directory. A `docker-compose.yaml` file utilizing pre-built images published in Docker Hub is prepared in the `/deploy/evaluation-framework/` directory to run the entire framework via `docker compose`.

Images are built for the `linux/amd64` platform and should work in Linux/Windows/MacOS environments with x86_64 architecture. For running the containers on ARM-based architectures, they need to be built using a different platform identifier. During thesis implementation, server with `Ubuntu 24 LTS` was used to run the containerized evaluation framework.

##### Prerequisites

- Docker
- Docker Compose
- API keys for remotely-accessed TR solutions (only if you want to use them)
- A `.env` file in the `deploy/evaluation-framework` directory containing following configurations:

```
// Mandatory
Jwt__Secret= <jwt secred used for signing jwt tokens by the evaluation service>
DB_USER= <usename for the postgre database used by evaluation service> // arbitrary
DB_PASSWORD= <password for the postgre database used by evaluation service> // arbitrary

// Optional but recommended
InitialUserCredentials__Username= <username for the initial evaluation tool user>
InitialUserCredentials__Password= <password for the initial evaluation tool user> // Min length 5

// Credentials for utilized text recognition solutions (not mandatory to run the framework)
ApiKeys__Gemini= <your gemini api key>
ApiKeys__AzureAiVision= <your azure ai vision api key>
ApiKeys__OpenAi= <your open ai api key>
ApiKeys__RunPod= <your runpod api key used for vlm self-hosting hunyuan and fine-tuned qwen>
ApiKeys__OpenRouter= <your open router api key used for baseline qwen model>
ApiKeys__GcpJsonCredentials= <your google cloud platform json credentials used for google ai vision>
```

##### Logging in

The evaluation service (`EvaluationTool`) implements JWT authentication to provide basic security (mainly against bot attacks when deployed on a server). As such, username/password authentication is required for accessing both `EvaluationTool` and `EvaluationTool.Ui`.

The evaluation service will automatically seed an initial user on the first migration, if you provide the necessary `InitialUserCredentials` environment variables/user secrets (see prerequisites for Docker Compose deployment).

To create users additionally, you can uncomment the `Register` endpoint in `src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.WebApi/User/Controllers/AuthController.cs` and use the `EvaluationTool` API.

##### Running

Once `.env` is prepared, ensure docker is running on your machine and go into the `deploy/evaluation-framework` directory. Then run the following command:

```
docker compose up
```

By default, only evaluation service and it's frontend will be mapped to local ports (10007 and 10008 respectively). This can be easily modified in the `docker-compose.yaml` configuration.

#### Aspire (for local development)

[.NET Aspire](https://aspire.dev/) was used throughout the implementation phase for running the evaluation framework in a local development environment. Local development was conducted on an ARM-based macOS device. Some of the framework's services depend on native packages (primarily the preprocessing services). Such dependencies proved problematic during development (especially when preparing linux-based server deployment) and despite best efforts to make the services compatible across CPU architectures and operating systems, some problems requiring updating project dependencies may still arrise (especially on Windows, which was not tested).

##### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Python >=`3.13.0` and <`4.0` (tested with `3.13.13`). Check by running `python --version` and `python3 --version` in CLI.
- Recommended IDE: JetBrains Rider with [Aspire plugin](https://plugins.jetbrains.com/plugin/23289-aspire)
- Docker
- Setting up required secrects
  - Same required secrets as specified in the Docker Compose prerequisites. Instead of placing these configurations in a dedicated `.env` file, they must be configured in `AiPipeline.TireOcr.Ocr.WebApi/appsettings.json` (for the TR solution API keys) and `AiPipeline.TireOcr.EvaluationTool.WebApi/appsettings.json` for all others.

##### Running

Make sure that docker is running on your machine before starting the Aspire project. If running through the JetBrains Rider IDE, use the `AiPipeline.Apphost:http` run configuration to start the Aspire dashboard and all constituent services. If running via CLI, go into the directory `src/AiPipeline.AppHost` and start Aspire via:

```
dotnet run
```

In both cases, Aspire should launch the dashboard on a free localhost port. For full experience, click on the URL of the `Frontend` service in the dashboard and log in via seeded user's credentials.

---

## Running Batches

The core functionality enabling the evaluation are the evaluation batches provided by the evaluation service (`EvaluationTool`), which ensure running TR pipeline with a specified preprocessing and recognition (OCR) configuration.

This functionality can be accessed either via `EvaluationTool` endpoint `/api/v1/Batch/Form`, or the frontend page `New Batch`. In both cases a CSV file containing URLs to images paired with the ground truth is necessary.

The format of the CSV file is as follows (example with 3 images):

```
https://example-image-storage.com/images/1.jpg,195-70R15C_104-102R
https://example-image-storage.com/images/2.jpg,235-55ZR17_103W
https://example-image-storage.com/images/3.jpg,225-65R17_106H
```

Each line contains 2 elements:

- URL leading to the desired input image of a tire
- Ground truth - the actual tire code present in the image specified as a string, where:
  - Slashes ('/') are replaced with '-'
  - Spaces (' ') are replaced with '\_'
  - Example: 215/65R16C 109/107T => 215-65R16C_109-107T

When running batches, you must specify:

- **Title** - display name of the batch results
- **Processing batch size** - how many pipeline inferences can run in parallel
- **Preprocessing** - variant of preprocessing to use
- **Ocr** - text recognition solution to use
- **Postprocessing** - type of preprocessing to use (currently only `SimpleExtractValues` is available)
- **DbMatching** - recommended to leave as `None`, as DbMatching process has no effect on evaluation

After starting, the individual pipeline instances of the batch are executed in the background. Results are available only after batch finishes, which, depending on the number of images in the batch, may take a long time. Progress indication of the batch is currently not implemented in UI, but can be deduced by observing logs of the evaluation service.

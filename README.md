# Tire Code Text Recognition Evaluation Framework

This archive contains the implementation of the **evaluation framework**, a core component of the thesis: "Evaluation and Application of Contemporary Text Recognition Solutions for Tire Parameter Extraction".

## Codebase Structure

The presented codebase follows a systematic structure. The following sections describe its fundamental components.

#### `deploy/`

The root `deploy` directory contains the scripts and configurations required to deploy the entire containerized evaluation framework using `docker compose`.

- **opencvsharp**: This subfolder contains shell scripts and a Dockerfile used to build a Docker image for the native dependencies of the `OpenCVSharp` library (a .NET wrapper for OpenCV). The resulting pre-built image is available on Docker Hub (`michalmusil/dotnet8-noble-opencv`) and is used during the image build for the .NET Preprocessing service.
- **evaluation-framework**: This subfolder contains shell scripts that, when executed from the archive root, build (`build-images.sh`) and publish (`publish-images.sh`) all the Docker images of the the evaluation framework. It also contains the `docker-compose.yaml` file, which serves as the deployment manifest.

#### `src/`

The root `src` directory contains the source code for all services within the evaluation framework. Each service is located in a dedicated subfolder named after the service.

The architecture of all services is based on the [Clean/Onion Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). Consequently, each service is structured into several projects/root folders: **Domain** (where applicable), **Application**, **Infrastructure**, and **Presentation** (with naming varying by technology).

---

## Thesis Terminology Differences

This codebase is the result of over a year of development and experimentation. As the evaluation framework evolved, certain discrepancies emerged between the terminology used in the code and the final thesis text.

### General Terms

- **OCR vs. Text Recognition**: While the more accurate term "Text Recognition" was ultimately used in the thesis text, the more ubiquitous "OCR" term was chosen in the early stages of the implementation. Consequently, the `Recognition` module and the recognition processes described in the thesis are referred to as `OCR` within this codebase.
- **Namespace Prefix**: Early design iterations utilized a different namespace structure. As a result, the now-redundant `AiPipeline` prefix is maintained across the service namespaces.

### Preprocessing Variants

The thesis focuses on the most promising preprocessing variants selected for evaluation. However, substantially more variants were experimentally investigated during implementation. These are differentiated by an enum located in `src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.Domain/StepTypes/PreprocessingType.cs`. The mapping of the preprocessing variant identifiers is as follows:

| Thesis Identifier | Codebase Enum Value (`PreprocessingType`) |
| :---------------- | :---------------------------------------- |
| **Variant 1**     | `ExtractAndComposeSlices`                 |
| **Variant 2**     | `ExtractTextsIntoMosaic`                  |
| **Variant 3**     | `ExtractAbsoluteRoi`                      |

### Text Recognition Solutions

Similarly, more TR solutions were experimentally investigated than were included in the final thesis evaluation. Within the framework, TR solutions are differentiated via an enum in `src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.Domain/StepTypes/OcrType.cs`. The mapping of the TR solution identifiers is as follows:

| Thesis Identifier        | Codebase Enum Value (`OcrType`) |
| :----------------------- | :------------------------------ |
| **Gemini 2.5 Flash**     | `GoogleGemini`                  |
| **GPT-4.1**              | `OpenAiGpt`                     |
| **Qwen3-VL 8B Instruct** | `QwenVl`                        |
| **HunyuanOCR**           | `HunyuanOcr`                    |
| **Google Cloud Vision**  | `GoogleCloudVision`             |
| **Azure AI Vision**      | `AzureAiVision`                 |
| **PP-OCRv5**             | `PaddleOcr`                     |
| **EasyOCR**              | `EasyOCR`                       |

---

## Services overview

<!-- - **AiPipeline.TireOcr.Preprocessing**: a .NET service providing the functionality for preprocessing variants 1 and 3 (and other experimental preprocessing variants not included in the evaluation).
- **AiPipeline.TireOcr.PythonPreprocessing**: a Python service providing the functionality for preprocessing variant 2 (and other experimental preprocessing variants not included in the evaluation).
- **AiPipeline.TireOcr.Ocr**: a .NET service facilitating the functionality of remote-accessed TR solutions (like Gemini, GPT, Qwen, HunyuanOCR, Google Ai Vision and Azure Ai Vision).
- **AiPipeline.TireOcr.PythonOcr**: a Python service facilitating the functionality locally hosted TR solutions (PP-OCRv5 and EasyOCR).
- **AiPipeline.TireOcr.Postprocessing**: a .NET service implementing the domain-specific postprocessing procedure.
- **AiPipeline.TireOcr.DbMatcher**: a .NET service implementing the tire code matching procedure included in the final production service prototype. This service isn't relevant for the evaluation process and was included as a part of this framework only in preparation for the production prototype implementation.
- **AiPipeline.TireOcr.EvaluationTool**: a .NET service implementing the functionality of the **Evaluation Service** described in the thesis evaluation framework design. This service acts as the core orchestrator of the evaluation framework, invoking pipelines and processing/storing the results.
- **AiPipeline.TireOcr.EvaluationTool.Ui**: a Vite React application serving as a front-end for the evaluation service. This SPA is written in typescript and provides a simplified interface for invoking pipelines/pipeline batches and analyzing/exporting the results.
- **AiPipeline.Shared**: a .NET class library project containing the shared core components used across all implemented .NET services.
- **AiPipeline.ServiceDefaults**: a .NET class library project providing fundamental configuration defaults used acrosss all implemented .NET services.
- **AiPipeline.AppHost**: a [.NET Aspire](https://aspire.dev/) project enabling seamless orchestration and running of the evaluation framework for development environments. -->

| Thesis Reference               | Service Name (Codebase)                  | Description                                                                                                                                                                                                                                                                            |
| :----------------------------- | :--------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Preprocessing Service (.NET)   | `AiPipeline.TireOcr.Preprocessing`       | A .NET service providing the functionality of preprocessing variants 1 and 3 (and other experimental variants not included in the evaluation).                                                                                                                                         |
| Preprocessing Service (Python) | `AiPipeline.TireOcr.PythonPreprocessing` | A Python service providing the functionality for preprocessing variant 2 (and other experimental variants not included in the evaluation).                                                                                                                                             |
| Recognition Service (.NET)     | `AiPipeline.TireOcr.Ocr`                 | A .NET service facilitating the functionality of remote-accessed TR solutions (Gemini 2.5 Flash, GPT-4.1, Qwen3-VL 8B Instruct, HunyuanOcr, Google Cloud Vision, and Azure Ai Vision).                                                                                                 |
| Recognition Service (Python)   | `AiPipeline.TireOcr.PythonOcr`           | A Python service facilitating the functionality of locally hosted TR solutions (PP-OCRv5 and EasyOcr).                                                                                                                                                                                 |
| Postprocessing Service         | `AiPipeline.TireOcr.Postprocessing`      | A .NET service implementing the domain-specific postprocessing procedure.                                                                                                                                                                                                              |
| Evaluation Service             | `AiPipeline.TireOcr.EvaluationTool`      | A .NET service acting as the core orchestrator, invoking pipelines and processing/storing results.                                                                                                                                                                                     |
| Evaluation Service Web UI      | `AiPipeline.TireOcr.EvaluationTool.Ui`   | A Vite React application serving as a front-end for the evaluation service. This SPA is written in TypeScript and provides an interface for invoking pipelines and analyzing results.                                                                                                  |
| -                              | `AiPipeline.TireOcr.DbMatcher`           | A .NET service implementing the tire code matching procedure included in the final production service prototype. This service is not relevant for the evaluation process and was included as a part of this framework only in preparation for the production prototype implementation. |
| -                              | `AiPipeline.Shared`                      | A .NET class library project containing the shared core components used across all implemented .NET services.                                                                                                                                                                          |
| -                              | `AiPipeline.ServiceDefaults`             | A .NET class library project providing fundamental configuration defaults used across all implemented .NET services.                                                                                                                                                                   |
| -                              | `AiPipeline.AppHost`                     | A [.NET Aspire](https://aspire.dev/) project enabling seamless orchestration and running of the evaluation framework for development environments.                                                                                                                                     |

---

## Deployment & Execution

### 1. Docker Compose (recommended for Evaluation)

All services are containerized. Each service directory contains a `Dockerfile`. A `docker-compose.yaml` file using pre-built images from Docker Hub is located in `/deploy/evaluation-framework/`.

Images are built for the `linux/amd64` platform (x86_64). For ARM-based architectures, images must be rebuilt with the appropriate platform identifier. The implementation was tested on a x86 server running `Ubuntu 24 LTS`.

#### Prerequisites

- Docker
- Docker Compose
- API keys for remote TR solutions (not necessary to start the framework)
- A `.env` file in the `deploy/evaluation-framework` directory with the following values specified:

```
# Mandatory Security Configuration [Evaluation Service]
Jwt__Secret= <jwt secred used for signing jwt tokens by the evaluation service>
DB_USER= <usename for the postgre database used by evaluation service> # arbitrary
DB_PASSWORD= <password for the postgre database used by evaluation service> # arbitrary

# Optional: Initial User Setup [Evaluation Service]
InitialUserCredentials__Username= <username for the initial evaluation service user>
InitialUserCredentials__Password= <password for the initial evaluation service user> # Min length 5

# API Keys (Required only for using specific TR solutions) [.NET Recognition service]
ApiKeys__Gemini= <your gemini api key>
ApiKeys__AzureAiVision= <your azure ai vision api key>
ApiKeys__OpenAi= <your open ai api key>
ApiKeys__RunPod= <your runpod api key used for vlm self-hosting hunyuan and fine-tuned qwen>
ApiKeys__OpenRouter= <your open router api key used for baseline qwen model>
ApiKeys__GcpJsonCredentials= <your gcp string json credentials used for google ai vision>
```

#### Logging In

The service automatically seeds an initial user during the first migration if `InitialUserCredentials` are provided (see `.env` contents). To create additional users, you may uncomment the `Register` endpoint in `src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.WebApi/User/Controllers/AuthController.cs`.

#### Execution

With the `.env` file prepared and Docker running, navigate to `deploy/evaluation-framework` and execute:

```bash
docker compose up
```

By default, the evaluation service and frontend are mapped to local ports 10007 and 10008, respectively.

### 2. .NET Aspire (Local Development)

[.NET Aspire](https://aspire.dev/) was used for local development, running on ARM-based macOS. Note that some of the preprocessing services' dependendies include native packages, which are installed differently depending on the platform. While efforts were made to ensure cross-platform compatibility of the entire evaluation framework, some dependencies may cause issues requiring dependency updates on untested environments, such as Windows.

#### Prerequisites

- Docker
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Python >= `3.13.0` and < `4.0` (tested with `3.13.13`).
  - Check by running `python --version` and `python3 --version` in CLI.
- Recommended IDE: JetBrains Rider with the [Aspire plugin](https://plugins.jetbrains.com/plugin/23289-aspire)
- Configuration (same values as specified in the Docker Compose `.env` prerequisites): Set secrets in `AiPipeline.TireOcr.Ocr.WebApi/appsettings.json` (for TR API keys) and `AiPipeline.TireOcr.EvaluationTool.WebApi/appsettings.json` (for all others).

#### Execution

Ensure Docker is running. When using JetBrains Rider, run the `AiPipeline.Apphost:http` configuration. When using dotnet CLI, navigate to `src/AiPipeline.AppHost` and run:

```bash
dotnet run
```

The Aspire dashboard will launch on a free localhost port. Access the `Frontend` service URL from the dashboard to log in.

---

## Running Evaluation Batches

Evaluation is driven by "evaluation batches" functionality provided by the evaluation service (`EvaluationTool`), which execute a TR pipeline with specific preprocessing and recognition configurations on multiple images.

This is accessible via the `/api/v1/Batch/Form` endpoint in evaluation service's API or the `New Batch` page in the frontend. A CSV file containing image URLs paired with ground truth labels is required.

**CSV Format (example with 2 images):**

```text
https://example-image-storage.com/images/1.jpg,195-70R15C_104-102R
https://example-image-storage.com/images/2.jpg,235-55ZR17_103W
```

**Ground Truth Formatting:**

- Replace slashes (`/`) with hyphens (`-`).
- Replace spaces (` `) with underscores (`_`).
- Example: `215/65R16C 109/107T` becomes `215-65R16C_109-107T`.

**Batch Parameters:**

- **Title**: Display name for the batch result.
- **Processing batch size**: Number of parallel pipeline inferences during batch execution.
- **Preprocessing**: The preprocessing variant to use.
- **Ocr**: The TR solution to use.
- **Postprocessing**: Currently only `SimpleExtractValues` is available.
- **DbMatching**: Set to `None` for evaluation purposes.

Batches run in the background. Results are available upon completion. While the UI does not currently show a progress indicator, progress can be monitored via the evaluation service logs.

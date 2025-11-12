# Tire OCR: AI Pipeline for Tire Code Extraction

This repository holds the complete codebase developed for my Master's thesis, focusing on **Optical Character Recognition (OCR) for Tire Parameter Extraction**.

The goal is to design, combine, and systematically evaluate several state-of-the-art AI and OCR approaches to build a robust, optimized pipeline for accurately reading **tire codes** (e.g., 205/55R16 95V) from tire sidewall photos.

---

## 🛠️ Project Structure and Architectural Overview

The repository is organized around a primary, actively developed tire code extraction pipeline (AiPipeline.TireOcr) and currently frozen orchestration project (AiPipeline.Orchestration). The actively developed pipeline is comprised of modular services connected via platform-independent APIs.

### Frozen Component: General AI Orchestration (`src/AiPipeline.Orchestration`)

This project, which is **no longer actively developed**, was onee of initial focuses of the thesis aimed at creating a universal, event-driven tool for running arbitrary AI workflows (using RabbitMQ and WolverineFx). It is retained purely for reference and context of the thesis's earlier evolutionary stages.

### Active Component: Tire OCR Pipeline (`src/AiPipeline.TireOcr`)

This branch contains the core solution for automating the extraction of tire parameters.

The problem is challenging due to real-world factors: **curved surfaces, low-contrast embossing, tire wear, variable lighting/angle, and cluttered backgrounds.** The pipeline is designed to overcome these challenges through a multi-stage approach, which is the subject of the thesis's comparative analysis.

The flow of the core pipeline steps is generalized in the following diagram:

![Flow of the individual steps of the TireOcr pipeline](/docs/flow.png)

---

## Modular Service Components

The Tire OCR pipeline is composed of the following distinct, Dockerized services:

| Service Name                                        | Purpose and Role in Pipeline                                    | Key Features                                                                                                                                                                    |
| :-------------------------------------------------- | :-------------------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Preprocessing** (`.Preprocessing`)                | Prepares raw images for OCR.                                    | ROI extraction, Dewarping/denoising, resizing, and slicing. Loads custom-trained ML models on startup.                                                                          |
| **OCR** (`.Ocr`)                                    | Aggregates and calls multiple public/cloud OCR/Vision/LLM APIs. | Supports **Gemini**, **Azure AI Vision**, **Mistral**, **OpenAI**, and **GC Vision** via configurable API keys.                                                                 |
| **PaddleOCR** (`.OcrPaddle`)                        | Dedicated OCR service using the **PaddleOCR** Python library.   | Packaged as a separate container for systematic comparison against cloud providers.                                                                                             |
| **Postprocessing** (`.Postprocessing`)              | Refines and validates raw OCR output.                           | Normalization, schema validation and value parsing.                                                                                                                             |
| **DB Matcher** (`.TasyDbMatcher`)                   | Matches the predicted tire code against external data.          | Web API for querying external tire/manufacturer databases (requires remote DB endpoints).                                                                                       |
| **Runner Prototype** (`.RunnerPrototype`)           | A simple prototype to test the end-to-end pipeline.             | Chained services (Preprocess → OCR → Postprocess → DB Match) via synchronous HTTP calls for initial trials.                                                                     |
| **Evaluation Tool (API)** (`.EvaluationTool`)       | Thesis component for comparative analysis.                      | ASP.NET Core API for configuring, executing, and orchestrating controlled evaluation runs/batches. Uses HttpClients with resilience. Persists results to PostgreSQL via EFCore. |
| **Evaluation Tool (UI/SPA)** (`.EvaluationTool.Ui`) | Frontend for managing and visualizing results.                  | React + Vite + TypeScript Single-Page Application (SPA).                                                                                                                        |

---

## Evaluation & Comparison Framework

The central aim of the thesis is achieved using the **Evaluation Tool**. This component orchestrates controlled batches of images through various pipeline configurations (e.g., different preprocessing steps, different OCR providers).

It measures and records **accuracy, duration, and robustness** to systematically compare variants and identify the optimal configuration for tire code extraction.

> **Note:** RESTful APIs were chosen for inter-service communication for simplicity in the thesis context. For a production environment, performance would be prioritized using a technology like gRPC.

---

## Technology Stack

| Category                     | Key Technologies Used                                                                                                                                            |
| :--------------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Backend**                  | **.NET 8, ASP.NET Core Web APIs**. Clean Architecture, CQRS, MediatR, and partial DDD. EF Core with **PostgreSQL**. **Dockerized** services.                     |
| **OCR Providers**            | Multi-API integration: Gemini, Azure AI Vision, Mistral, OpenAI, GC Vision. Dedicated PaddleOCR service and more likely comming in the future.                   |
| **Frontend (Evaluation UI)** | React, Vite, TypeScript. Styling with TailwindCSS and `shadcn/ui`. Simple state management using TanStack Query. Form handling with `react-hook-form` and `zod`. |
| **Deployment & Tooling**     | Docker and Docker Compose for deployments. .NET Aspire for streamlined local development and debugging.                                                          |

---

## 📦 Deployment Instructions

Two pre-configured `docker-compose` setups are provided for different use cases. Relevant images for either setup can be built using the respective `build-images.sh` script.

### 1. Prototype Pipeline (No UI)

This setup is ideal for testing the core processing path and includes all core services aggregated under the **RunnerPrototype** API.

```yaml
deploy/prototype/docker-compose.yaml
```

Includes: Preprocessing, Ocr, OcrPaddle, Postprocessing, TasyDbMatcher, and RunnerPrototype.

### 2. Full Evaluation Stack (API + UI)

This setup is used for conducting systematic experiments and includes the full front-end and back-end for managing runs.

```yaml
deploy/evaluation-tool/docker-compose.yaml
```

Includes: All prototype services plus the EvaluationTool (API) and the EvaluationToolUi (SPA).

**⚠️ Platform Warning:** Some Docker images have deep dependencies on platform-specific libraries. Running on Linux is recommended to avoid potential compatibility issues.

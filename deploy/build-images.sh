#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_preprocessing-service"
OCR_IMAGE_NAME="michalmusil/tire-ocr_ocr-service"
POSTPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_postprocessing-service"
TIRE_DB_MATCHER_IMAGE_NAME="michalmusil/tire-ocr_tire-db-matcher-service"
RUNNER_PROTOTYPE_IMAGE_NAME="michalmusil/tire-ocr_runner-prototype"

# PREPROCESSING
echo "Building image for preprocessing service"
docker build --platform linux/amd64 -t "${PREPROCESSING_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.WebApi/Dockerfile .
echo "Successfully built ${PREPROCESSING_IMAGE_NAME}"
echo ""

# OCR
echo "Building image for ocr service"
docker build --platform linux/amd64 -t "${OCR_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.Ocr/AiPipeline.TireOcr.Ocr.WebApi/Dockerfile .
echo "Successfully built ${OCR_IMAGE_NAME}"
echo ""

# POSTPROCESSING
echo "Building image for postprocessing service"
docker build --platform linux/amd64 -t "${POSTPROCESSING_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/Dockerfile .
echo "Successfully built ${POSTPROCESSING_IMAGE_NAME}"
echo ""

# TIRE_DB_MATCHER
echo "Building image for tire db matcher service"
docker build --platform linux/amd64 -t "${TIRE_DB_MATCHER_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/Dockerfile .
echo "Successfully built ${TIRE_DB_MATCHER_IMAGE_NAME}"
echo ""

# RUNNER_PROTOTYPE
echo "Building image for runner prototype"
docker build --platform linux/amd64 -t "${RUNNER_PROTOTYPE_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.RunnerPrototype/Dockerfile .
echo "Successfully built ${RUNNER_PROTOTYPE_IMAGE_NAME}"
echo ""

echo "All docker images built successfully."
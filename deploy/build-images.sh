#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="tire-ocr/preprocessing-service"
OCR_IMAGE_NAME="tire-ocr/ocr-service"
POSTPROCESSING_IMAGE_NAME="tire-ocr/postprocessing-service"
RUNNER_PROTOTYPE_IMAGE_NAME="tire-ocr/runner-prototype"

# PREPROCESSING
echo "Building image for preprocessing service"
docker build --platform linux/amd64 -t "${PREPROCESSING_IMAGE_NAME}:latest" -f src/Preprocessing/TireOcr.Preprocessing.WebApi/Dockerfile .
echo "Successfully built ${PREPROCESSING_IMAGE_NAME}"
echo ""

# OCR
echo "Building image for ocr service"
docker build --platform linux/amd64 -t "${OCR_IMAGE_NAME}:latest" -f src/Ocr/TireOcr.Ocr.WebApi/Dockerfile .
echo "Successfully built ${OCR_IMAGE_NAME}"
echo ""

# POSTPROCESSING
echo "Building image for postprocessing service"
docker build --platform linux/amd64 -t "${POSTPROCESSING_IMAGE_NAME}:latest" -f src/Preprocessing/TireOcr.Preprocessing.WebApi/Dockerfile .
echo "Successfully built ${POSTPROCESSING_IMAGE_NAME}"
echo ""

# RUNNER_PROTOTYPE
echo "Building image for runner prototype"
docker build --platform linux/amd64 -t "${RUNNER_PROTOTYPE_IMAGE_NAME}:latest" -f src/TireOcr.RunnerPrototype/Dockerfile .
echo "Successfully built ${RUNNER_PROTOTYPE_IMAGE_NAME}"
echo ""

echo "All docker images built successfully."
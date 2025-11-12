#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_preprocessing-service"
OCR_IMAGE_NAME="michalmusil/tire-ocr_ocr-service"
POSTPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_postprocessing-service"
TIRE_DB_MATCHER_IMAGE_NAME="michalmusil/tire-ocr_tire-db-matcher-service"
RUNNER_PROTOTYPE_IMAGE_NAME="michalmusil/tire-ocr_runner-prototype"

# PREPROCESSING
echo "Pushing image for preprocessing service"
docker push "${PREPROCESSING_IMAGE_NAME}:latest"
echo ""

# OCR
echo "Pushing image for ocr service"
docker push "${OCR_IMAGE_NAME}:latest"
echo ""

# POSTPROCESSING
echo "Pushing image for postprocessing service"
docker push "${POSTPROCESSING_IMAGE_NAME}:latest"
echo ""

# TIRE_DB_MATCHER
echo "Pushing image for tire db matcher service"
docker push "${TIRE_DB_MATCHER_IMAGE_NAME}:latest"
echo ""

# RUNNER_PROTOTYPE
echo "Pushing image for runner prototype"
docker push "${RUNNER_PROTOTYPE_IMAGE_NAME}:latest"
echo ""

echo "All docker images pushed to Docker Hub."
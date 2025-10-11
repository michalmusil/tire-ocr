#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_preprocessing-service"
OCR_IMAGE_NAME="michalmusil/tire-ocr_ocr-service"
OCR_PADDLE_IMAGE_NAME="michalmusil/tire-ocr_ocr-paddle-service"
POSTPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_postprocessing-service"
TIRE_DB_MATCHER_IMAGE_NAME="michalmusil/tire-ocr_tire-db-matcher-service"
EVALUATION_TOOL_IMAGE_NAME="michalmusil/tire-ocr_evaluation-tool"

# PREPROCESSING
echo "Pushing image for preprocessing service"
docker push "${PREPROCESSING_IMAGE_NAME}:latest"
echo ""


# OCR
echo "Pushing image for ocr service"
docker push "${OCR_IMAGE_NAME}:latest"
echo ""

# OCR PADDLE
echo "Pushing image for ocr paddle service"
docker push "${OCR_PADDLE_IMAGE_NAME}:latest"
echo ""

# POSTPROCESSING
echo "Pushing image for postprocessing service"
docker push "${POSTPROCESSING_IMAGE_NAME}:latest"
echo ""

# TIRE_DB_MATCHER
echo "Pushing image for tire db matcher service"
docker push "${TIRE_DB_MATCHER_IMAGE_NAME}:latest"
echo ""

# EVALUATION_TOOL
echo "Pushing image for evaluation tool"
docker push "${EVALUATION_TOOL_IMAGE_NAME}:latest"
echo ""

echo "All docker images pushed to Docker Hub."
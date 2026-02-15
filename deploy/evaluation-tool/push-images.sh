#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_eval_preprocessing-service"
PREPROCESSING_PYTHON_IMAGE_NAME="michalmusil/tire-ocr_eval_preprocessing-python-service"
OCR_IMAGE_NAME="michalmusil/tire-ocr_eval_ocr-service"
OCR_PYTHON_IMAGE_NAME="michalmusil/tire-ocr_eval_ocr-python-service"
POSTPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_eval_postprocessing-service"
TIRE_DB_MATCHER_IMAGE_NAME="michalmusil/tire-ocr_tire-db-matcher-service"
EVALUATION_TOOL_IMAGE_NAME="michalmusil/tire-ocr_evaluation-tool"
EVALUATION_TOOL_FE_IMAGE_NAME="michalmusil/tire-ocr_evaluation-tool-fe"

# PREPROCESSING
echo "Pushing image for preprocessing service"
docker push "${PREPROCESSING_IMAGE_NAME}:latest"
echo ""

# PREPROCESSING PYTHON
echo "Pushing image for preprocessing python service"
docker push "${PREPROCESSING_PYTHON_IMAGE_NAME}:latest"
echo ""

# OCR
echo "Pushing image for ocr service"
docker push "${OCR_IMAGE_NAME}:latest"
echo ""

# OCR PYTHON
echo "Pushing image for ocr python service"
docker push "${OCR_PYTHON_IMAGE_NAME}:latest"
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

# EVALUATION_TOOL_FE
echo "Pushing image for evaluation tool frontend"
docker push "${EVALUATION_TOOL_FE_IMAGE_NAME}:latest"
echo ""

echo "All docker images pushed to Docker Hub."
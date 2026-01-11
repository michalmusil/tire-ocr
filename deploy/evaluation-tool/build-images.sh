#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_eval_preprocessing-service"
OCR_IMAGE_NAME="michalmusil/tire-ocr_eval_ocr-service"
OCR_PYTHON_IMAGE_NAME="michalmusil/tire-ocr_eval_ocr-python-service"
POSTPROCESSING_IMAGE_NAME="michalmusil/tire-ocr_eval_postprocessing-service"
TIRE_DB_MATCHER_IMAGE_NAME="michalmusil/tire-ocr_tire-db-matcher-service"
EVALUATION_TOOL_IMAGE_NAME="michalmusil/tire-ocr_evaluation-tool"
EVALUATION_TOOL_FE_IMAGE_NAME="michalmusil/tire-ocr_evaluation-tool-fe"

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

# OCR_PYTHON
echo "Building image for ocr python service"
docker buildx create --use --name mybuilder
docker buildx inspect --bootstrap
docker build --platform linux/amd64 -t "${OCR_PYTHON_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/Dockerfile .
echo "Successfully built ${OCR_PADDLE_IMAGE_NAME}"
echo ""

# POSTPROCESSING
echo "Building image for postprocessing service"
docker build --platform linux/amd64 -t "${POSTPROCESSING_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/Dockerfile .
echo "Successfully built ${POSTPROCESSING_IMAGE_NAME}"
echo ""

# EVALUATION_TOOL
echo "Building image for evaluation tool"
docker build --platform linux/amd64 -t "${EVALUATION_TOOL_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.WebApi/Dockerfile .
echo "Successfully built ${EVALUATION_TOOL_IMAGE_NAME}"
echo ""

# EVALUATION_TOOL_FE
echo "Building image for evaluation tool frontend"
docker build --platform linux/amd64 -t "${EVALUATION_TOOL_FE_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.EvaluationTool.Ui/Dockerfile ./src/AiPipeline.TireOcr/AiPipeline.TireOcr.EvaluationTool.Ui
echo "Successfully built ${EVALUATION_TOOL_FE_IMAGE_NAME}"
echo ""

# TIRE_DB_MATCHER
echo "Building image for tire db matcher service"
docker build --platform linux/amd64 -t "${TIRE_DB_MATCHER_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr/AiPipeline.TireOcr.TasyDbMatcher/AiPipeline.TireOcr.TasyDbMatcher.WebApi/Dockerfile .
echo "Successfully built ${TIRE_DB_MATCHER_IMAGE_NAME}"
echo ""

echo "All docker images built successfully."

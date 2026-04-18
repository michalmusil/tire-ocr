#!/bin/bash

set -e

PREPROCESSING_IMAGE_NAME="michalmusil/tirecode-evaluation-preprocessing"
PREPROCESSING_PYTHON_IMAGE_NAME="michalmusil/tirecode-evaluation-preprocessing-python"
RECOGNITION_IMAGE_NAME="michalmusil/tirecode-evaluation-recognition"
RECOGNITION_PYTHON_IMAGE_NAME="michalmusil/tirecode-evaluation-recognition-python"
POSTPROCESSING_IMAGE_NAME="michalmusil/tirecode-evaluation-postprocessing"
TIRE_DB_MATCHER_IMAGE_NAME="michalmusil/tirecode-evaluation-dbmatcher"
EVALUATION_SERVICE_IMAGE_NAME="michalmusil/tirecode-evaluation-service"
EVALUATION_SERVICE_FE_IMAGE_NAME="michalmusil/tirecode-evaluation-service-fe"

# PREPROCESSING
echo "Building image for preprocessing service"
docker build --platform linux/amd64 -t "${PREPROCESSING_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.Preprocessing/AiPipeline.TireOcr.Preprocessing.WebApi/Dockerfile .
echo "Successfully built ${PREPROCESSING_IMAGE_NAME}"
echo ""

# PREPROCESSING_PYTHON
echo "Building image for preprocessing python service"
docker buildx create --use --name mybuilder
docker buildx inspect --bootstrap
docker build --platform linux/amd64 -t "${PREPROCESSING_PYTHON_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.PythonPreprocessing/Dockerfile .
echo "Successfully built ${PREPROCESSING_PYTHON_IMAGE_NAME}"
echo ""

# RECOGNITION
echo "Building image for recognition service"
docker build --platform linux/amd64 -t "${RECOGNITION_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.Ocr/AiPipeline.TireOcr.Ocr.WebApi/Dockerfile .
echo "Successfully built ${RECOGNITION_IMAGE_NAME}"
echo ""

# RECOGNITION_PYTHON
echo "Building image for recognition python service"
#docker buildx create --use --name mybuilder
docker buildx inspect --bootstrap
docker build --platform linux/amd64 -t "${RECOGNITION_PYTHON_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.PythonOcr/Dockerfile .
echo "Successfully built ${RECOGNITION_PYTHON_IMAGE_NAME}"
echo ""

# POSTPROCESSING
echo "Building image for postprocessing service"
docker build --platform linux/amd64 -t "${POSTPROCESSING_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.Postprocessing/AiPipeline.TireOcr.Postprocessing.WebApi/Dockerfile .
echo "Successfully built ${POSTPROCESSING_IMAGE_NAME}"
echo ""

# EVALUATION_SERVICE
echo "Building image for evaluation service"
docker build --platform linux/amd64 -t "${EVALUATION_SERVICE_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.EvaluationTool/AiPipeline.TireOcr.EvaluationTool.WebApi/Dockerfile .
echo "Successfully built ${EVALUATION_SERVICE_IMAGE_NAME}"
echo ""

# EVALUATION_SERVICE_FE
echo "Building image for evaluation service frontend"
docker build --platform linux/amd64 -t "${EVALUATION_SERVICE_FE_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.EvaluationTool.Ui/Dockerfile ./src/AiPipeline.TireOcr.EvaluationTool.Ui
echo "Successfully built ${EVALUATION_SERVICE_FE_IMAGE_NAME}"
echo ""

# TIRE_DB_MATCHER
echo "Building image for tire db matcher service"
docker build --platform linux/amd64 -t "${TIRE_DB_MATCHER_IMAGE_NAME}:latest" -f src/AiPipeline.TireOcr.DbMatcher/AiPipeline.TireOcr.DbMatcher.WebApi/Dockerfile .
echo "Successfully built ${TIRE_DB_MATCHER_IMAGE_NAME}"
echo ""

echo "All docker images built successfully."

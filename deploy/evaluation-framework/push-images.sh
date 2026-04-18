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
echo "Pushing image for preprocessing service"
docker push "${PREPROCESSING_IMAGE_NAME}:latest"
echo ""

# PREPROCESSING PYTHON
echo "Pushing image for preprocessing python service"
docker push "${PREPROCESSING_PYTHON_IMAGE_NAME}:latest"
echo ""

# RECOGNITION
echo "Pushing image for recognition service"
docker push "${RECOGNITION_IMAGE_NAME}:latest"
echo ""

# RECOGNITION PYTHON
echo "Pushing image for recognition python service"
docker push "${RECOGNITION_PYTHON_IMAGE_NAME}:latest"
echo ""

# POSTPROCESSING
echo "Pushing image for postprocessing service"
docker push "${POSTPROCESSING_IMAGE_NAME}:latest"
echo ""

# TIRE_DB_MATCHER
echo "Pushing image for tire db matcher service"
docker push "${TIRE_DB_MATCHER_IMAGE_NAME}:latest"
echo ""

# EVALUATION_SERVICE
echo "Pushing image for evaluation service"
docker push "${EVALUATION_SERVICE_IMAGE_NAME}:latest"
echo ""

# EVALUATION_SERVICE_FE
echo "Pushing image for evaluation service frontend"
docker push "${EVALUATION_SERVICE_FE_IMAGE_NAME}:latest"
echo ""

echo "All docker images pushed to Docker Hub."
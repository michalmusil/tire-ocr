#!/bin/bash

set -e

IMAGE_NAME="michalmusil/dotnet8-noble-opencv"

echo "Pushing opencv image"
docker push "${IMAGE_NAME}:latest"
echo "Opencv image pushed to docker hub"
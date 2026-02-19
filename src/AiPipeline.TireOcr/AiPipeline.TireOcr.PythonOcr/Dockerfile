FROM amd64/python:3.13.8-slim-bookworm as build

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    build-essential \
    ffmpeg libsm6 libxext6 libgl1 \
    git \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*


RUN pip install poetry==2.2.1

ENV POETRY_NO_INTERACTION=1 \
    POETRY_VIRTUALENVS_IN_PROJECT=1 \
    POETRY_VIRTUALENVS_CREATE=1 \
    POETRY_CACHE_DIR=/tmp/poetry_cache

WORKDIR /app

COPY src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/pyproject.toml ./
COPY src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/main.py ./
COPY src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/src ./src
RUN touch README.md

RUN poetry install --no-root && rm -rf $POETRY_CACHE_DIR


FROM amd64/python:3.13.8-slim-bookworm as final

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    ffmpeg libsm6 libxext6 libgl1 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*


ENV VIRTUAL_ENV=/app/.venv \
    PATH="/app/.venv/bin:$PATH"

WORKDIR /app
COPY --from=build ${VIRTUAL_ENV} ${VIRTUAL_ENV}
COPY src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/main.py ./
COPY src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/custom_models ./custom_models
COPY src/AiPipeline.TireOcr/AiPipeline.TireOcr.PythonOcr/src ./src
EXPOSE 10100

# Pre-download PaddleOCR models during the build
RUN python -c "from paddleocr import PaddleOCR; PaddleOCR(use_textline_orientation=True, lang='en', text_recognition_model_name='PP-OCRv5_mobile_rec')"

ENTRYPOINT ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "10100"]


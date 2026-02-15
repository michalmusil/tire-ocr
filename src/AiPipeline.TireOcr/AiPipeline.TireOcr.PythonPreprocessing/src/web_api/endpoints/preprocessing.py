from typing import Annotated
from fastapi import APIRouter, Depends, File, UploadFile, HTTPException
from fastapi.responses import StreamingResponse, JSONResponse
import base64

from src.dependencies import get_perform_preprocessing_3_command_handler

from ...application.commands.perform_preprocessing_3.perform_preprocessing_3_command import (
    PerformPreprocessing3Command,
)
from ...application.commands.perform_preprocessing_3.perform_preprocessing_3_command_handler import (
    PerformPreprocessing3CommandHandler,
)

router = APIRouter(prefix="/preprocessing", tags=["Preprocessing"])


@router.post("/v3/file")
# add annotation for UploadFile
async def preprocessing_v3_file(
    image: Annotated[
        UploadFile, File(description="Image file to perform the preprocessing on.")
    ],
    handler: Annotated[
        PerformPreprocessing3CommandHandler,
        Depends(get_perform_preprocessing_3_command_handler),
    ],
):
    try:
        image_bytes = await image.read()
        command = PerformPreprocessing3Command(image=image_bytes)
        result = await handler.handle(command)

        if result.status == "error":
            return JSONResponse(
                status_code=400,
                content={
                    "status": result.status,
                    "message": result.message,
                    "duration_ms": result.duration_ms,
                },
            )

        return StreamingResponse(iter([result.image]), media_type="image/png")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@router.post("/v3")
async def preprocessing_v3_file_json(
    image: Annotated[
        UploadFile, File(description="Image file to perform the preprocessing on.")
    ],
    handler: Annotated[
        PerformPreprocessing3CommandHandler,
        Depends(get_perform_preprocessing_3_command_handler),
    ],
):
    try:
        image_bytes = await image.read()
        command = PerformPreprocessing3Command(image=image_bytes)
        result = await handler.handle(command)

        if result.status == "error":
            return JSONResponse(
                status_code=400,
                content={
                    "status": result.status,
                    "message": result.message,
                    "duration_ms": result.duration_ms,
                },
            )

        base64_image = base64.b64encode(result.image).decode("utf-8")
        return JSONResponse(
            content={
                "FileName": image.filename,
                "ContentType": image.content_type or "application/octet-stream",
                "Base64ImageData": base64_image,
                "DurationMs": result.duration_ms,
            }
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

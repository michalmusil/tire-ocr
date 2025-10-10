from fastapi import APIRouter, Depends, File, UploadFile, HTTPException, status
from typing import Annotated

from src.application.dtos.ocr_result_dto import OCRResult
from src.application.services.ocr_service import OcrService
from src.dependencies import get_ocr_service
from src.web_api.contracts.ocr.ocr_response import OCRResponse


router = APIRouter(
    prefix="/ocr",
    tags=["OCR Extraction"],
)


@router.post(
    "/paddle",
    response_model=OCRResponse,
    summary="Perform Tire OCR on an uploaded image.",
    description="Accepts an image file via multipart/form-data and returns recognized text if found, 404 otherwise.",
)
async def perform_paddle_ocr(
    image: Annotated[
        UploadFile, File(description="Image file to perform Tire OCR on.")
    ],
    ocr_service: Annotated[OcrService, Depends(get_ocr_service)],
) -> OCRResult:
    content_type: str = image.content_type if image.content_type else ""
    if not content_type.startswith("image/"):
        raise HTTPException(
            status_code=status.HTTP_422_UNPROCESSABLE_CONTENT,
            detail=f"Invalid file type. Expected an image, got {content_type}",
        )
    image_bytes: bytes = await image.read()

    if not image_bytes:
        raise HTTPException(
            status_code=status.HTTP_422_UNPROCESSABLE_CONTENT,
            detail="The uploaded image file is empty.",
        )

    ocr_result: OCRResult = await ocr_service.perform_tire_ocr(image_bytes)

    match ocr_result.status:
        case "not_found":
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail="Tire OCR failed to detect any text in the image",
            )
        case "error":
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail=f"Tire OCR failed: internal server error",
            )

    return OCRResponse(
        detectedCode=ocr_result.extracted_text,
        DurationMs=ocr_result.duration_ms,
    )

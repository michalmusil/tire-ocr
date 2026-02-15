from fastapi import APIRouter, Depends, File, UploadFile, HTTPException, status, Form
from typing import Annotated, Optional

from src.application.dtos.ocr_result_dto import OCRResult
from src.application.services.ocr_service import OcrService
from src.dependencies import get_paddle_ocr_service, get_easy_ocr_service
from src.web_api.contracts.ocr.ocr_response import OCRResponse


router = APIRouter(
    prefix="/ocr",
    tags=["OCR Extraction"],
)


@router.post(
    "/paddle",
    response_model=OCRResponse,
    summary="Perform Tire OCR (PaddleOCR) on an uploaded image.",
    description="Accepts an image file via multipart/form-data and returns recognized text if found, 404 otherwise. Optionally decomposes vertically stacked image slices.",
)
async def perform_paddle_ocr(
    image: Annotated[
        UploadFile, File(description="Image file to perform Tire OCR on.")
    ],
    ocr_service: Annotated[OcrService, Depends(get_paddle_ocr_service)],
    number_of_vertical_stacked_slices: Annotated[
        Optional[int],
        Form(
            description="Optional: Number of vertical slices to decompose the image into. If provided, the image will be split vertically and OCR performed on each slice separately.",
            ge=1,
            le=20,
        ),
    ] = None,
) -> OCRResponse:
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
    ocr_result: OCRResult

    ocr_result = await ocr_service.perform_tire_ocr(
        image_bytes, number_of_vertical_stacked_slices
    )

    match ocr_result.status:
        case "not_found":
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=ocr_result.message,
            )
        case "error":
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail=ocr_result.message,
            )

    return OCRResponse(
        detectedCode=ocr_result.extracted_text,
        DurationMs=ocr_result.duration_ms,
    )


@router.post(
    "/easy",
    response_model=OCRResponse,
    summary="Perform Tire OCR (EasyOCR) on an uploaded image.",
    description="Accepts an image file via multipart/form-data and returns recognized text if found, 404 otherwise. Optionally decomposes vertically stacked image slices.",
)
async def perform_easy_ocr(
    image: Annotated[
        UploadFile, File(description="Image file to perform Tire OCR on.")
    ],
    ocr_service: Annotated[OcrService, Depends(get_easy_ocr_service)],
    number_of_vertical_stacked_slices: Annotated[
        Optional[int],
        Form(
            description="Optional: Number of vertical slices to decompose the image into. If provided, the image will be split vertically and OCR performed on each slice separately.",
            ge=1,
            le=20,
        ),
    ] = None,
) -> OCRResponse:
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

    ocr_result: OCRResult = await ocr_service.perform_tire_ocr(
        image_bytes, number_of_vertical_stacked_slices
    )

    match ocr_result.status:
        case "not_found":
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=ocr_result.message,
            )
        case "error":
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail=ocr_result.message,
            )

    return OCRResponse(
        detectedCode=ocr_result.extracted_text,
        DurationMs=ocr_result.duration_ms,
    )

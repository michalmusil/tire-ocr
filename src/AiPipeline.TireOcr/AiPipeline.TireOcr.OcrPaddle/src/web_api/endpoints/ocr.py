from fastapi import APIRouter, Depends, File, UploadFile, HTTPException, status, Form
from typing import Annotated, Optional, List
import io
from PIL import Image

from src.application.dtos.ocr_result_dto import OCRResult
from src.application.services.ocr_service import OcrService
from src.dependencies import get_ocr_service
from src.web_api.contracts.ocr.ocr_response import OCRResponse


router = APIRouter(
    prefix="/ocr",
    tags=["OCR Extraction"],
)


def slice_image_vertically(image_bytes: bytes, num_slices: int) -> List[bytes]:
    """
    Slice an image vertically into multiple equal-height slices.
    """
    if num_slices < 1:
        raise ValueError("Number of slices must be at least 1")

    try:
        image = Image.open(io.BytesIO(image_bytes))
        width, height = image.size

        slice_height = height // num_slices

        if slice_height == 0:
            raise ValueError(
                "Image height is too small to create the requested number of slices"
            )

        slices = []
        for i in range(num_slices):
            top = i * slice_height
            bottom = (i + 1) * slice_height if i < num_slices - 1 else height
            slice_image = image.crop((0, top, width, bottom))

            slice_bytes_io = io.BytesIO()
            slice_image.save(slice_bytes_io, format=image.format or "PNG")
            slice_bytes = slice_bytes_io.getvalue()
            slices.append(slice_bytes)

        return slices

    except Exception as e:
        raise ValueError(f"Failed to slice image: {str(e)}")


@router.post(
    "/paddle",
    response_model=OCRResponse,
    summary="Perform Tire OCR on an uploaded image.",
    description="Accepts an image file via multipart/form-data and returns recognized text if found, 404 otherwise. Optionally decomposes vertically stacked image slices.",
)
async def perform_paddle_ocr(
    image: Annotated[
        UploadFile, File(description="Image file to perform Tire OCR on.")
    ],
    ocr_service: Annotated[OcrService, Depends(get_ocr_service)],
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

    # Process multiple vertically stacked slices
    if number_of_vertical_stacked_slices is not None:
        try:
            image_slices = slice_image_vertically(
                image_bytes, number_of_vertical_stacked_slices
            )
        except ValueError as e:
            raise HTTPException(
                status_code=status.HTTP_422_UNPROCESSABLE_CONTENT,
                detail=str(e),
            )

        ocr_result: OCRResult = await ocr_service.perform_tire_ocr_on_slices(
            image_slices
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
    # Process a single image
    else:
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

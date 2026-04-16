from typing import Annotated
from fastapi import Depends
from src.application.services.image_slicer_service import ImageSlicerService
from src.application.services.ocr_service import OcrService
from src.infrastructure.services.paddle_ocr_service import PaddleOcrService
from src.infrastructure.services.easy_ocr_service import EasyOcrService
from src.infrastructure.services.image_slicer_service_impl import ImageSlicerServiceImpl


_image_slicer_service_instance: ImageSlicerService = ImageSlicerServiceImpl()
_paddle_ocr_service_instance: OcrService = PaddleOcrService(
    _image_slicer_service_instance
)
_easy_ocr_service_instance: OcrService = EasyOcrService(_image_slicer_service_instance)


def get_image_slicer_service() -> ImageSlicerService:
    return _image_slicer_service_instance


def get_paddle_ocr_service() -> OcrService:
    return _paddle_ocr_service_instance


def get_easy_ocr_service() -> OcrService:
    return _easy_ocr_service_instance

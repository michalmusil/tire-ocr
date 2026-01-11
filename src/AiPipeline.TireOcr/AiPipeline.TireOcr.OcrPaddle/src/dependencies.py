from src.application.services.ocr_service import OcrService
from src.infrastructure.services.paddle_ocr_service import PaddleOcrService
from src.infrastructure.services.easy_ocr_service import EasyOcrService


_paddle_ocr_service_instance: OcrService = PaddleOcrService()
_easy_ocr_service_instance: OcrService = EasyOcrService()


def get_paddle_ocr_service() -> OcrService:
    return _paddle_ocr_service_instance


def get_easy_ocr_service() -> OcrService:
    return _easy_ocr_service_instance

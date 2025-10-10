from src.application.services.ocr_service import OcrService
from src.infrastructure.services.paddle_ocr_service import PaddleOcrService


ocr_service_instance: OcrService = PaddleOcrService()


def get_ocr_service() -> OcrService:
    return ocr_service_instance

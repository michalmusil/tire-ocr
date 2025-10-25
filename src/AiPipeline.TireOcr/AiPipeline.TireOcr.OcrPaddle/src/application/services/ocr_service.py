from abc import ABC, abstractmethod

from src.application.dtos.ocr_result_dto import OCRResult


class OcrService(ABC):
    @abstractmethod
    async def perform_tire_ocr(self, image_bytes: bytes) -> OCRResult:
        pass

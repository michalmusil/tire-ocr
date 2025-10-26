from abc import ABC, abstractmethod
from typing import List

from src.application.dtos.ocr_result_dto import OCRResult


class OcrService(ABC):
    @abstractmethod
    async def perform_tire_ocr(self, image_bytes: bytes) -> OCRResult:
        pass

    @abstractmethod
    async def perform_tire_ocr_on_slices(self, image_slices: List[bytes]) -> OCRResult:
        pass

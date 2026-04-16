from abc import ABC, abstractmethod
from typing import List

from src.application.dtos.ocr_result_dto import OCRResult


class OcrService(ABC):
    @abstractmethod
    async def perform_tire_ocr(
        self, image_bytes: bytes, number_of_vertical_stacked_slices: int | None = None
    ) -> OCRResult:
        pass

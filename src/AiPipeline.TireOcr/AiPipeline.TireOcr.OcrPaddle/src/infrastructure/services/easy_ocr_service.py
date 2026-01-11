import io
import time
from typing import Optional, Any, List
import easyocr

import numpy as np
from PIL import Image
from dotenv import load_dotenv

from src.application.services.ocr_service import OCRResult, OcrService


load_dotenv()
EASY_OCR_ENGINE: Optional[easyocr.Reader] = None


def get_easy_ocr_engine() -> easyocr.Reader:
    global EASY_OCR_ENGINE
    if EASY_OCR_ENGINE is None:
        print("EasyOCR: INITIALIZING")
        EASY_OCR_ENGINE = easyocr.Reader(["en"], gpu=False)
        print("EasyOCR: INITIALIZED")
    return EASY_OCR_ENGINE


class EasyOcrService(OcrService):
    reader: easyocr.Reader

    def __init__(self) -> None:
        self.reader = get_easy_ocr_engine()

    async def perform_tire_ocr(self, image_bytes: bytes) -> OCRResult:
        start_time = time.perf_counter()
        try:
            image: Image.Image = Image.open(io.BytesIO(image_bytes)).convert("RGB")
            image_np: np.ndarray = np.array(image)

            result: Any = self.reader.readtext(image_np, text_threshold=0.3)
            detected_texts: List[str] = []
            if isinstance(result, list) and len(result) > 0:
                for item in result:
                    if isinstance(item, (list, tuple)) and len(item) >= 2:
                        text = item[1]
                        if isinstance(text, str):
                            detected_texts.append(text)

            detected_text: str = " ".join(detected_texts).strip()
            duration = int((time.perf_counter() - start_time) * 1000)

            if detected_text:
                return OCRResult(
                    status="success",
                    message="EasyOCR was successful.",
                    extracted_text=detected_text,
                    duration_ms=duration,
                )
            else:
                return OCRResult(
                    status="not_found",
                    message="Easy OCR failed to detect any text in the image",
                    extracted_text=None,
                    duration_ms=duration,
                )
        except Exception as e:
            error_msg = f"An internal error occurred during EasyOCR processing: {e.__class__.__name__}: {str(e)}"
            print(error_msg)
            duration = int((time.perf_counter() - start_time) * 1000)
            return OCRResult(
                status="error",
                message=error_msg,
                extracted_text=None,
                duration_ms=duration,
            )

    async def perform_tire_ocr_on_slices(self, image_slices: List[bytes]) -> OCRResult:
        combined_text_parts: List[str] = []
        total_duration_ms = 0

        for i, slice_bytes in enumerate(image_slices):
            ocr_result: OCRResult = await self.perform_tire_ocr(slice_bytes)
            total_duration_ms += ocr_result.duration_ms

            if ocr_result.status == "success" and ocr_result.extracted_text:
                if "/" in ocr_result.extracted_text:
                    combined_text_parts.append(ocr_result.extracted_text)
            elif ocr_result.status == "error":
                return OCRResult(
                    status="error",
                    message=f"Tire OCR failed on slice {i}: {ocr_result.message}",
                    extracted_text=None,
                    duration_ms=total_duration_ms,
                )

        if not combined_text_parts:
            return OCRResult(
                status="not_found",
                message="Tire OCR failed to detect any text in any of the image slices",
                extracted_text=None,
                duration_ms=total_duration_ms,
            )

        combined_text = " ".join(combined_text_parts)

        return OCRResult(
            status="success",
            message=f"Tire OCR was successful on {len(combined_text_parts)} out of {len(image_slices)} slices.",
            extracted_text=combined_text,
            duration_ms=total_duration_ms,
        )

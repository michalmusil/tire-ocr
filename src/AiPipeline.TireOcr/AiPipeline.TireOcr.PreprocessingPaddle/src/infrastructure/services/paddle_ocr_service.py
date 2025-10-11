import io
import time
import numpy as np
from PIL import Image
from paddleocr import PaddleOCR

from typing import Optional, Any
from src.application.services.ocr_service import OCRResult, OcrService

PADDLE_OCR_ENGINE: Optional[PaddleOCR] = None


def get_paddle_ocr_engine() -> PaddleOCR:
    global PADDLE_OCR_ENGINE
    if PADDLE_OCR_ENGINE is None:
        print("PaddleOCR: INITIALIZING")
        PADDLE_OCR_ENGINE = PaddleOCR(use_textline_orientation=True, lang="en")
        print("PaddleOCR: INITIALIZED")
    return PADDLE_OCR_ENGINE


class PaddleOcrService(OcrService):
    ocr_engine: PaddleOCR

    def __init__(self) -> None:
        self.ocr_engine = get_paddle_ocr_engine()

    async def perform_tire_ocr(self, image_bytes: bytes) -> OCRResult:
        start_time = time.perf_counter()

        try:
            image: Image.Image = Image.open(io.BytesIO(image_bytes)).convert("RGB")
            image_np: np.ndarray = np.array(image)
            result: Any = self.ocr_engine.predict(image_np)

            detected_text_list: list[str] = []

            result_valid = result and isinstance(result, list) and len(result) > 0
            if result_valid:
                for page_result in result:
                    if not "rec_texts" in page_result:
                        continue
                    recognized_texts = page_result["rec_texts"]
                    detected_text_list.extend(recognized_texts)

            detected_text: str = "\n".join(detected_text_list).strip()
            duration = int((time.perf_counter() - start_time) * 1000)

            if detected_text:
                return OCRResult(
                    status="success",
                    message=f"Tire OCR was successful.",
                    extracted_text=detected_text,
                    duration_ms=duration,
                )
            else:
                return OCRResult(
                    status="not_found",
                    message="Tire OCR failed to detect any text in the image",
                    extracted_text=None,
                    duration_ms=duration,
                )
        except Exception as e:
            error_msg: str = (
                f"An internal error occurred during OCR processing: {e.__class__.__name__}: {str(e)}"
            )
            print(error_msg)
            duration = int((time.perf_counter() - start_time) * 1000)
            return OCRResult(
                status="error",
                message=error_msg,
                extracted_text=None,
                duration_ms=duration,
            )

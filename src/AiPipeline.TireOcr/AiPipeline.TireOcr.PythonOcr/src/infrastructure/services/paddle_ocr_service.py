import io
import time
import numpy as np
from PIL import Image
from paddleocr import PaddleOCR
from dotenv import load_dotenv


from typing import Optional, Any, List
from src.application.services.ocr_service import OCRResult, OcrService

load_dotenv()
PADDLE_OCR_ENGINE: Optional[PaddleOCR] = None


def get_paddle_ocr_engine() -> PaddleOCR:
    global PADDLE_OCR_ENGINE
    if PADDLE_OCR_ENGINE is None:
        print("PaddleOCR: INITIALIZING")
        model_name = "PP-OCRv5_mobile_rec"
        model_dir = "./custom_models/rec/PP-OCRv5_mobile_rec_200e"

        PADDLE_OCR_ENGINE = PaddleOCR(
            # use_doc_orientation_classify=False,
            # use_doc_unwarping=False,
            lang="en",
            # text_recognition_model_name=model_name,
            # text_recognition_model_dir=model_dir,
        )
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

            detected_text: str = " ".join(detected_text_list).strip()
            duration = int((time.perf_counter() - start_time) * 1000)

            if detected_text:
                return OCRResult(
                    status="success",
                    message=f"PaddleOCR was successful.",
                    extracted_text=detected_text,
                    duration_ms=duration,
                )
            else:
                return OCRResult(
                    status="not_found",
                    message="PaddleOCR failed to detect any text in the image",
                    extracted_text=None,
                    duration_ms=duration,
                )
        except Exception as e:
            error_msg: str = (
                f"An internal error occurred during PaddleOCR processing: {e.__class__.__name__}: {str(e)}"
            )
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
                # Only append if extracted text contains '/' character
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

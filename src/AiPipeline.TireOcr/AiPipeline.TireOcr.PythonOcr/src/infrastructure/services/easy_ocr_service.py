import io
import time
from typing import Optional, Any, List
import easyocr
import threading

from fastapi.concurrency import run_in_threadpool
import numpy as np
from PIL import Image
from dotenv import load_dotenv

from src.application.services.image_slicer_service import ImageSlicerService
from src.application.services.ocr_service import OCRResult, OcrService


load_dotenv()
EASY_OCR_ENGINE: Optional[easyocr.Reader] = None
easy_ocr_lock = threading.Lock()


def get_easy_ocr_engine() -> easyocr.Reader:
    global EASY_OCR_ENGINE
    if EASY_OCR_ENGINE is None:
        print("EasyOCR: INITIALIZING")
        EASY_OCR_ENGINE = easyocr.Reader(["en"], gpu=False)
        print("EasyOCR: INITIALIZED")
    return EASY_OCR_ENGINE


class EasyOcrService(OcrService):
    reader: easyocr.Reader

    def __init__(self, image_slicer_service: ImageSlicerService) -> None:
        self.reader = get_easy_ocr_engine()
        self.image_slicer_service = image_slicer_service

    async def perform_tire_ocr(
        self, image_bytes: bytes, number_of_vertical_stacked_slices: int | None = None
    ) -> OCRResult:
        start_time = time.perf_counter()
        try:
            image_slices: List[bytes] = []
            if number_of_vertical_stacked_slices is not None:
                image_slices = self.image_slicer_service.slice_image_vertically(
                    image_bytes, number_of_vertical_stacked_slices
                )
            else:
                image_slices.append(image_bytes)

            combined_texts: List[str] = []
            for image_slice in image_slices:
                result = await run_in_threadpool(self.run_inference, image_slice)
                combined_texts.extend(result)

            detected_text: str = " ".join(combined_texts).strip()
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

    def run_inference(self, image_bytes: bytes) -> List[str]:
        processing_start_time = time.perf_counter()

        print("Starting EasyOCR of image")
        image_np: np.ndarray
        with Image.open(io.BytesIO(image_bytes)) as image:
            rgb = image.convert("RGB")
            image_np = np.array(rgb)
        with easy_ocr_lock:
            result = self.reader.readtext(image_np, text_threshold=0.3)

        processing_end_time = time.perf_counter()
        print(
            f"Finished EasyOCR of image. Time: {processing_end_time - processing_start_time}"
        )

        detected_texts: List[str] = []
        if isinstance(result, list) and len(result) > 0:
            for item in result:
                if isinstance(item, (list, tuple)) and len(item) >= 2:
                    text = item[1]
                    if isinstance(text, str):
                        detected_texts.append(text)
        return detected_texts


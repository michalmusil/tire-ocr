import io
import time
from fastapi.concurrency import run_in_threadpool
import numpy as np
from PIL import Image
from paddleocr import PaddleOCR
from dotenv import load_dotenv
import threading

from typing import Optional, List
from src.application.services.image_slicer_service import ImageSlicerService
from src.application.services.ocr_service import OCRResult, OcrService

load_dotenv()
PADDLE_OCR_ENGINE: Optional[PaddleOCR] = None
paddle_lock = threading.Lock()


def get_paddle_ocr_engine() -> PaddleOCR:
    global PADDLE_OCR_ENGINE
    if PADDLE_OCR_ENGINE is None:
        print("PaddleOCR: INITIALIZING")
        model_name = "PP-OCRv5_mobile_rec"
        model_dir = "./custom_models/rec/3_15_epochs_with_synth_with_tire_dict"

        PADDLE_OCR_ENGINE = PaddleOCR(
#             use_doc_orientation_classify=False,
#             use_textline_orientation=False,
#             use_doc_unwarping=False,
            lang="en",
            text_recognition_model_name=model_name,
#             text_recognition_model_dir=model_dir,
        )
        print("PaddleOCR: INITIALIZED")
    return PADDLE_OCR_ENGINE


class PaddleOcrService(OcrService):
    ocr_engine: PaddleOCR

    def __init__(self, image_slicer_service: ImageSlicerService) -> None:
        self.ocr_engine = get_paddle_ocr_engine()
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

            combined_texts: list[str] = []

            for image_slice in image_slices:
                result = await run_in_threadpool(self.run_inference, image_slice)
                combined_texts.extend(result)

            detected_text: str = " ".join(combined_texts).strip()
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

    def run_inference(self, image_bytes: bytes) -> List[str]:
        processing_start_time = time.perf_counter()

        print("Starting PaddleOCR of image")
        image_np: np.ndarray
        with Image.open(io.BytesIO(image_bytes)) as image:
            rgb = image.convert("RGB")
            image_np = np.array(rgb)
        with paddle_lock:
            result = self.ocr_engine.predict(image_np)

        processing_end_time = time.perf_counter()
        print(
            f"Finished PaddleOCR of image. Time: {processing_end_time - processing_start_time}"
        )

        detected_text_list: list[str] = []
        result_valid = result and isinstance(result, list) and len(result) > 0
        if result_valid:
            for page_result in result:
                if not "rec_texts" in page_result:
                    continue
                recognized_texts = page_result["rec_texts"]
                detected_text_list.extend(recognized_texts)
        return detected_text_list

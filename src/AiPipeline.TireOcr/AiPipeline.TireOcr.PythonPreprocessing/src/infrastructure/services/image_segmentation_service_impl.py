import cv2
import numpy as np
from ...application.services.image_segmentation_service import ImageSegmentationService
from paddleocr import TextDetection
from typing import Optional

TEXT_DETECTION_ENGINE: Optional[TextDetection] = None


def get_text_detection_engine() -> TextDetection:
    global TEXT_DETECTION_ENGINE
    if TEXT_DETECTION_ENGINE is None:
        print("TextDetection: INITIALIZING")
        TEXT_DETECTION_ENGINE = TextDetection(model_name="PP-OCRv5_server_det")
        print("Paddle TextDetection: INITIALIZED")
    return TEXT_DETECTION_ENGINE


class ImageSegmentationServiceImpl(ImageSegmentationService):
    async def emphasise_characters(self, image_bytes: bytes) -> bytes:
        # Placeholder: simple contrast boost to emulate emphasising characters
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
        if img is None:
            return image_bytes
        lab = cv2.cvtColor(img, cv2.COLOR_BGR2LAB)
        l, a, b = cv2.split(lab)
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
        cl = clahe.apply(l)
        limg = cv2.merge((cl, a, b))
        enhanced = cv2.cvtColor(limg, cv2.COLOR_LAB2BGR)
        _, out_bytes = cv2.imencode(".png", enhanced)
        return out_bytes.tobytes()

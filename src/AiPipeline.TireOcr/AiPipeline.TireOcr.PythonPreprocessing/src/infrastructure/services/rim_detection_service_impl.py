from src.application.services.rim_detection_service import RimDetectionService
from ultralytics import YOLO
import numpy as np
import cv2
import os
from typing import Optional

_YOLO_MODEL: Optional[YOLO] = None


def _get_model() -> YOLO:
    global _YOLO_MODEL
    if _YOLO_MODEL is not None:
        return _YOLO_MODEL
    model_path = os.path.abspath(
        os.path.join(
            os.getcwd(),
            "models/tire_segmentation_v5.onnx",
        )
    )
    _YOLO_MODEL = YOLO(model_path, task="segment")
    return _YOLO_MODEL


class RimDetectionServiceImpl(RimDetectionService):
    def __init__(self) -> None:
        self.model = _get_model()

    def detect_rim(self, image_bytes: bytes) -> tuple[int, int, float]:
        # Decode image
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
        if img is None:
            raise ValueError("Invalid image data")

        # Run segmentation (model is a segmentation ONNX; no need to pass task here)
        results = self.model.predict(
            img, imgsz=640, conf=0.25, verbose=False, device="cpu"
        )
        if not results:
            raise ValueError("No rim detected")

        res = results[0]
        if res.masks is None or res.boxes is None:
            raise ValueError("No rim detected")

        names = self.model.names  # dict: {class_id: class_name}
        boxes = res.boxes
        masks = res.masks

        cls = boxes.cls.cpu().numpy().astype(int)
        conf = boxes.conf.cpu().numpy().astype(float)

        # Filter only detections labeled 'rim'
        rim_indices = []
        for i, c in enumerate(cls):
            label = names[c] if isinstance(names, dict) and c in names else str(c)
            if str(label).lower() == "rim":
                rim_indices.append(i)

        if not rim_indices:
            raise ValueError("No rim detected")

        # Pick best by confidence
        best_i = max(rim_indices, key=lambda i: conf[i])

        # Build binary mask in original image space
        h, w = img.shape[:2]
        pts = masks.xy[best_i]
        pts = np.round(pts).astype(np.int32)
        mask_np = np.zeros((h, w), dtype=np.uint8)
        cv2.fillPoly(mask_np, [pts], 1)

        if np.count_nonzero(mask_np) == 0:
            raise ValueError("No rim detected")

        contours, _ = cv2.findContours(
            mask_np, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
        )
        if not contours:
            raise ValueError("No rim detected")

        largest = max(contours, key=cv2.contourArea)
        (cx, cy), radius = cv2.minEnclosingCircle(largest)

        # Ensure circle is within bounds (optional safety)
        if not (
            0 <= cx - radius
            and cx + radius <= w
            and 0 <= cy - radius
            and cy + radius <= h
        ):
            raise ValueError("Detected rim circle is not within image bounds")

        return int(cx), int(cy), float(radius)

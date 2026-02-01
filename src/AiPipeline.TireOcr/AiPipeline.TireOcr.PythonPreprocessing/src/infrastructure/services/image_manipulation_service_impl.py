import cv2
import numpy as np
from src.application.services.image_manipulation_service import ImageManipulationService


class CvImageManipulationService(ImageManipulationService):
    async def resize_to_max_dimension(
        self, image_bytes: bytes, max_dimension: int
    ) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
        h, w, _ = img.shape
        if max(h, w) <= max_dimension:
            return image_bytes
        scale = max_dimension / max(h, w)
        new_w, new_h = int(w * scale), int(h * scale)
        resized = cv2.resize(img, (new_w, new_h), interpolation=cv2.INTER_AREA)
        _, out_bytes = cv2.imencode(".png", resized)
        return out_bytes.tobytes()

    async def perform_clahe(self, image_bytes: bytes) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        clahe = cv2.createCLAHE(clipLimit=40, tileGridSize=(8, 8))
        cl = clahe.apply(gray)

        _, out_bytes = cv2.imencode(".png", cl)
        return out_bytes.tobytes()

    async def unwarp_tire_rim(
        self,
        image_bytes: bytes,
        center_x: int,
        center_y: int,
        inner_radius: float,
        outer_radius: float,
    ) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
        center = (int(center_x), int(center_y))
        flags = cv2.WARP_POLAR_LINEAR + cv2.WARP_FILL_OUTLIERS

        cropped_width = int(outer_radius)
        cropped_height = int(2 * np.pi * inner_radius)

        polar = cv2.warpPolar(
            img,
            (cropped_width, cropped_height),
            center,
            outer_radius,
            flags,
        )
        tire_thickness = int(outer_radius - inner_radius)
        cropped_result = polar[:, cropped_width - tire_thickness :]
        rotated_result = cv2.rotate(cropped_result, cv2.ROTATE_90_COUNTERCLOCKWISE)

        left_slice = rotated_result[:, : int(rotated_result.shape[1] * 0.17)]
        prolonged_result = cv2.hconcat([rotated_result, left_slice])

        h, w, _ = prolonged_result.shape
        mid = w // 2
        left = prolonged_result[:, :mid]
        right = prolonged_result[:, mid:]
        larger_width = max(left.shape[1], right.shape[1])
        left = self._ensure_slice_width(left, larger_width)
        right = self._ensure_slice_width(right, larger_width)

        stacked = np.vstack((left, right))
        _, out_bytes = cv2.imencode(".png", stacked)
        return out_bytes.tobytes()

    def _ensure_slice_width(self, img: np.ndarray, target_size: int) -> np.ndarray:
        if img.shape[1] >= target_size:
            return img

        return cv2.copyMakeBorder(
            img,
            0,
            0,
            0,
            target_size - img.shape[1],
            cv2.BORDER_CONSTANT,
            value=0,
        )

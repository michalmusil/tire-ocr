import cv2
import numpy as np
from src.application.services.image_manipulation_service import ImageManipulationService


class ImageManipulationServiceImpl(ImageManipulationService):
    def ensure_grayscale(self, image: np.ndarray) -> np.ndarray:
        if image.ndim == 3:
            return cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        return image

    def resize_to_max_dimension(
        self, image: np.ndarray, max_dimension: int
    ) -> np.ndarray:
        h, w = image.shape[:2]
        if max(h, w) <= max_dimension:
            return image
        scale = max_dimension / max(h, w)
        new_w, new_h = int(w * scale), int(h * scale)
        resized = cv2.resize(image, (new_w, new_h), interpolation=cv2.INTER_AREA)
        return resized

    def image_to_bytes(self, image: np.ndarray) -> bytes:
        _, buffer = cv2.imencode(".png", image)
        return buffer.tobytes()

    def perform_clahe(self, image: np.ndarray) -> np.ndarray:
        gray = self.ensure_grayscale(image)
        clahe = cv2.createCLAHE(clipLimit=40, tileGridSize=(5, 5))
        cl = clahe.apply(gray)
        return cl

    def perform_bilateral_filter(self, image: np.ndarray) -> np.ndarray:
        gray = self.ensure_grayscale(image)
        filtered = cv2.bilateralFilter(gray, 5, 40, 40)
        return filtered

    def perform_bitwise_not(self, image: np.ndarray) -> np.ndarray:
        inverted = cv2.bitwise_not(image)
        return inverted

    def perform_sobel_edge_detection(self, image: np.ndarray) -> np.ndarray:
        gray = self.ensure_grayscale(image)

        grad_x = cv2.Sobel(gray, cv2.CV_16S, 1, 0, ksize=3)
        grad_y = cv2.Sobel(gray, cv2.CV_16S, 0, 1, ksize=3)
        abs_x = cv2.convertScaleAbs(grad_x)
        abs_y = cv2.convertScaleAbs(grad_y)
        result = cv2.addWeighted(abs_x, 0.5, abs_y, 0.5, 0)
        return result

    def copy_and_append_image_portion_from_left(
        self, image: np.ndarray, append_width_ratio: float
    ) -> np.ndarray:
        gray = self.ensure_grayscale(image)
        _, width = gray.shape[:2]
        appendix_width = int(width * append_width_ratio)

        if appendix_width <= 0:
            raise ValueError(
                "Image size and append_width_ratio must result in a width > 0"
            )

        left_slice = gray[:, : int(appendix_width)]
        prolonged_result = cv2.hconcat([gray, left_slice])
        return prolonged_result

    def unwarp_tire_rim(
        self,
        image: np.ndarray,
        center_x: int,
        center_y: int,
        inner_radius: float,
        outer_radius: float,
    ) -> np.ndarray:
        center = (int(center_x), int(center_y))
        flags = cv2.WARP_POLAR_LINEAR + cv2.WARP_FILL_OUTLIERS

        cropped_width = int(outer_radius)
        cropped_height = int(2 * np.pi * inner_radius)

        polar = cv2.warpPolar(
            image,
            (cropped_width, cropped_height),
            center,
            outer_radius,
            flags,
        )
        tire_thickness = int(outer_radius - inner_radius)
        cropped_result = polar[:, cropped_width - tire_thickness :]
        rotated_result = cv2.rotate(cropped_result, cv2.ROTATE_90_COUNTERCLOCKWISE)
        return rotated_result

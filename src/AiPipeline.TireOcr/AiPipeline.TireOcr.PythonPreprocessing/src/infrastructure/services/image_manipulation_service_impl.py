import cv2
import numpy as np
from src.application.services.image_manipulation_service import ImageManipulationService


class ImageManipulationServiceImpl(ImageManipulationService):
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
        clahe = cv2.createCLAHE(clipLimit=40, tileGridSize=(5, 5))
        cl = clahe.apply(gray)

        _, out_bytes = cv2.imencode(".png", cl)
        return out_bytes.tobytes()

    async def perform_bilateral_filter(self, image_bytes: bytes) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_GRAYSCALE)
        filtered = cv2.bilateralFilter(img, 5, 40, 40)
        _, out_bytes = cv2.imencode(".png", filtered)
        return out_bytes.tobytes()

    async def perform_bitwise_not(self, image_bytes: bytes) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_GRAYSCALE)
        inverted = cv2.bitwise_not(img)
        _, out_bytes = cv2.imencode(".png", inverted)
        return out_bytes.tobytes()

    async def perform_sobel_edge_detection(self, image_bytes: bytes) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        gray_image = cv2.imdecode(np_arr, cv2.IMREAD_GRAYSCALE)

        grad_x = cv2.Sobel(gray_image, cv2.CV_16S, 1, 0, ksize=3)
        grad_y = cv2.Sobel(gray_image, cv2.CV_16S, 0, 1, ksize=3)
        abs_x = cv2.convertScaleAbs(grad_x)
        abs_y = cv2.convertScaleAbs(grad_y)
        result = cv2.addWeighted(abs_x, 0.5, abs_y, 0.5, 0)

        _, out_bytes = cv2.imencode(".png", result)
        return out_bytes.tobytes()

    async def copy_and_append_image_portion_from_left(
        self, image_bytes: bytes, append_width_ratio: float
    ) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        image = cv2.imdecode(np_arr, cv2.IMREAD_GRAYSCALE)
        height, width = image.shape[:2]
        appendix_width = int(width * append_width_ratio)

        if appendix_width <= 0:
            raise ValueError(
                "Image size and append_width_ratio must result in a width > 0"
            )

        left_slice = image[:, : int(appendix_width)]
        prolonged_result = cv2.hconcat([image, left_slice])

        _, out_bytes = cv2.imencode(".png", prolonged_result)
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

        _, out_bytes = cv2.imencode(".png", rotated_result)
        return out_bytes.tobytes()

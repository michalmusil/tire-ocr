from typing import List, Optional, Tuple
import cv2
import numpy as np

from src.application.services.image_slicer_service import ImageSlicerService


class ImageSlicerServiceImpl(ImageSlicerService):
    def slice_image_with_additive_overlap(
        self,
        image: np.ndarray,
        slice_size: Tuple[int, int],
        x_overlap_ratio: float,
        y_overlap_ratio: float,
    ) -> Optional[List[np.ndarray]]:
        """
        Slices an image into smaller parts with additive overlap based on a byte array input.

        Args:
            image_bytes: The raw byte array of the input image.
            slice_size: A tuple containing (width, height) of the slice.
            x_overlap_ratio: The ratio of width to add as overlap (e.g., 0.1 for 10%).
            y_overlap_ratio: The ratio of height to add as overlap (e.g., 0.1 for 10%).

        Returns:
            A list of byte arrays for each slice, or None if an error occurs.
        """
        try:
            if image.ndim == 3:
                input_image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
            else:
                input_image = image

            height, width = input_image.shape
            slice_w, slice_h = slice_size

            overlap_w = x_overlap_ratio * slice_w
            overlap_h = y_overlap_ratio * slice_h

            starting_xs = np.arange(0, width, slice_w)
            starting_ys = np.arange(0, height, slice_h)
            result_slices: List[np.ndarray] = []

            for y in starting_ys:
                for x in starting_xs:
                    xmin = int(max(0, x - overlap_w))
                    ymin = int(max(0, y - overlap_h))
                    xmax = int(min(x + slice_w + overlap_w, width))
                    ymax = int(min(y + slice_h + overlap_h, height))

                    slice = input_image[ymin:ymax, xmin:xmax]
                    result_slices.append(slice)

            return result_slices

        except Exception as e:
            print(f"Error 500: Failed to slice image. Details: {e}")
            return None

    def stack_images_vertically(
        self,
        images: List[np.ndarray],
    ) -> Optional[np.ndarray]:
        if not images:
            return None

        largest_width = max([img.shape[1] for img in images])
        normalized_images = [
            self._ensure_slice_width(img, largest_width) for img in images
        ]

        stacked = np.vstack(normalized_images)
        return stacked

    def _ensure_slice_width(
        self,
        img: np.ndarray,
        target_size: int,
    ) -> np.ndarray:
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

from abc import ABC, abstractmethod
import numpy as np


class ImageManipulationService(ABC):
    @abstractmethod
    def ensure_grayscale(self, image: np.ndarray) -> np.ndarray:
        pass

    @abstractmethod
    def resize_to_max_dimension(
        self, image: np.ndarray, max_dimension: int
    ) -> np.ndarray:
        pass

    @abstractmethod
    def image_to_bytes(self, image: np.ndarray) -> bytes:
        pass

    @abstractmethod
    def perform_clahe(self, image: np.ndarray) -> np.ndarray:
        pass

    @abstractmethod
    def perform_bilateral_filter(self, image: np.ndarray) -> np.ndarray:
        pass

    @abstractmethod
    def perform_bitwise_not(self, image: np.ndarray) -> np.ndarray:
        pass

    @abstractmethod
    def perform_sobel_edge_detection(self, image: np.ndarray) -> np.ndarray:
        pass

    @abstractmethod
    def copy_and_append_image_portion_from_left(
        self, image: np.ndarray, append_width_ratio: float
    ) -> np.ndarray:
        pass

    @abstractmethod
    def unwarp_tire_rim(
        self,
        image: np.ndarray,
        center_x: int,
        center_y: int,
        inner_radius: float,
        outer_radius: float,
    ) -> np.ndarray:
        pass

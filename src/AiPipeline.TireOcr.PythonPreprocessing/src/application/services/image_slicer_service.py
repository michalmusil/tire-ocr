from abc import ABC, abstractmethod
from typing import List, Optional, Tuple
import numpy as np


class ImageSlicerService(ABC):
    @abstractmethod
    def slice_image_with_additive_overlap(
        self,
        image: np.ndarray,
        slice_size: Tuple[int, int],
        x_overlap_ratio: float,
        y_overlap_ratio: float,
    ) -> Optional[List[np.ndarray]]:
        pass

    @abstractmethod
    def stack_images_vertically(
        self,
        images: List[np.ndarray],
    ) -> Optional[np.ndarray]:
        pass

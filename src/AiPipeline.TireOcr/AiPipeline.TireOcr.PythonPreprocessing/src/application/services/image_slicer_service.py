from abc import ABC, abstractmethod
from typing import List, Optional, Tuple


class ImageSlicerService(ABC):
    @abstractmethod
    def slice_image_with_additive_overlap(
        self,
        image_bytes: bytes,
        slice_size: Tuple[int, int],
        x_overlap_ratio: float,
        y_overlap_ratio: float,
    ) -> Optional[List[bytes]]:
        pass

    @abstractmethod
    def stack_images_vertically(
        self,
        images: List[bytes],
    ) -> Optional[bytes]:
        pass

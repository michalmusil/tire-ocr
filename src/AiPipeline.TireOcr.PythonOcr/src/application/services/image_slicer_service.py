from abc import ABC, abstractmethod
from typing import List


class ImageSlicerService(ABC):
    @abstractmethod
    def slice_image_vertically(image_bytes: bytes, num_slices: int) -> List[bytes]:
        pass

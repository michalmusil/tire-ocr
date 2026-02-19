from abc import ABC, abstractmethod
import numpy as np


class ImageSegmentationService(ABC):
    @abstractmethod
    def emphasise_characters_on_text_regions(self, image: np.ndarray) -> np.ndarray:
        pass

    @abstractmethod
    def compose_emphasised_text_region_mosaic(
        self, image: np.ndarray, emphasise_characters: bool
    ) -> np.ndarray:
        pass

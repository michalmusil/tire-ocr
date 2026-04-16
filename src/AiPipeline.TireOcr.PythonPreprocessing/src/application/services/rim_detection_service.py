from abc import ABC, abstractmethod
import numpy as np


class RimDetectionService(ABC):
    @abstractmethod
    def detect_rim(
        self, image: np.ndarray
    ) -> tuple[int, int, float]:  # returns center_x, center_y, radius
        pass

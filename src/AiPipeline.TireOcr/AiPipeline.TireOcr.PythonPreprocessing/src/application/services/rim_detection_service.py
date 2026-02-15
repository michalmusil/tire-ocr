from abc import ABC, abstractmethod


class RimDetectionService(ABC):
    @abstractmethod
    def detect_rim(
        self, image_bytes: bytes
    ) -> tuple[int, int, float]:  # returns center_x, center_y, radius
        pass

from abc import ABC, abstractmethod


class ImageManipulationService(ABC):
    @abstractmethod
    async def resize_to_max_dimension(
        self, image_bytes: bytes, max_dimension: int
    ) -> bytes:
        pass

    @abstractmethod
    async def perform_clahe(self, image_bytes: bytes) -> bytes:
        pass

    @abstractmethod
    async def unwarp_tire_rim(
        self,
        image_bytes: bytes,
        center_x: int,
        center_y: int,
        inner_radius: float,
        outer_radius: float,
    ) -> bytes:
        pass

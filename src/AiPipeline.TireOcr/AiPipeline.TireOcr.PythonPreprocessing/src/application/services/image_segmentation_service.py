from abc import ABC, abstractmethod


class ImageSegmentationService(ABC):
    @abstractmethod
    async def emphasise_characters(self, image_bytes: bytes) -> bytes:
        pass

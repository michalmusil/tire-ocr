from abc import ABC, abstractmethod


class ImageSegmentationService(ABC):
    @abstractmethod
    async def emphasise_characters_on_text_regions(self, image_bytes: bytes) -> bytes:
        pass

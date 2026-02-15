from abc import ABC, abstractmethod


class ImageSegmentationService(ABC):
    @abstractmethod
    def emphasise_characters_on_text_regions(self, image_bytes: bytes) -> bytes:
        pass

    @abstractmethod
    def compose_emphasised_text_region_mosaic(
        self, image_bytes: bytes, emphasise_characters: bool
    ) -> bytes:
        pass

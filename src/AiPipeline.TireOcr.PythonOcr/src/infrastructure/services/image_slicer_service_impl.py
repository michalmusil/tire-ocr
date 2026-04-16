from src.application.services.image_slicer_service import ImageSlicerService
from PIL import Image
import io


class ImageSlicerServiceImpl(ImageSlicerService):
    def slice_image_vertically(
        self, image_bytes: bytes, num_slices: int
    ) -> list[bytes]:
        """
        Slice an image vertically into multiple equal-height slices.
        """
        if num_slices < 1:
            raise ValueError("Number of slices must be at least 1")

        try:
            slices = []
            with Image.open(io.BytesIO(image_bytes)) as image:
                width, height = image.size

                slice_height = height // num_slices

                if slice_height == 0:
                    raise ValueError(
                        "Image height is too small to create the requested number of slices"
                    )
                for i in range(num_slices):
                    top = i * slice_height
                    bottom = (i + 1) * slice_height if i < num_slices - 1 else height
                    slice_image = image.crop((0, top, width, bottom))

                    slice_bytes_io = io.BytesIO()
                    slice_image.save(slice_bytes_io, format=image.format or "PNG")
                    slice_bytes = slice_bytes_io.getvalue()
                    slices.append(slice_bytes)

            return slices

        except Exception as e:
            raise ValueError(f"Failed to slice image: {str(e)}")

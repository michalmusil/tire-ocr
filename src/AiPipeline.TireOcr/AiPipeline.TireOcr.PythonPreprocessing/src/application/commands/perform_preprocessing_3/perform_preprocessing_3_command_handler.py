from ..perform_preprocessing_3.perform_preprocessing_3_command import (
    PerformPreprocessing3Command,
)
from ...services.image_manipulation_service import ImageManipulationService
from ...services.rim_detection_service import RimDetectionService
from ...services.image_segmentation_service import ImageSegmentationService
from ...dtos.preprocessing_result_dto import PreprocessingResultDto
import time


class PerformPreprocessing3CommandHandler:
    def __init__(
        self,
        image_manipulation_service: ImageManipulationService,
        rim_detection_service: RimDetectionService,
        image_segmentation_service: ImageSegmentationService,
    ):
        self.image_manipulation_service = image_manipulation_service
        self.rim_detection_service = rim_detection_service
        self.image_segmentation_service = image_segmentation_service

    async def handle(
        self, command: PerformPreprocessing3Command
    ) -> PreprocessingResultDto:
        start = time.perf_counter()
        try:
            # 1) Resize to max dimension
            resized_image = (
                await self.image_manipulation_service.resize_to_max_dimension(
                    command.image, 2048
                )
            )

            # 2) Rim detection (may throw if not found)
            try:
                center_x, center_y, radius = (
                    await self.rim_detection_service.detect_rim(resized_image)
                )
            except Exception as ex:
                duration_ms = int((time.perf_counter() - start) * 1000)
                return PreprocessingResultDto(
                    status="acceptable_failure",
                    message=f"Rim detection failed: {str(ex)}",
                    image=resized_image,
                    duration_ms=duration_ms,
                )
            inner_radius = radius * 0.9
            outer_radius = radius * 1.3

            # 3) Unwarp tire rim to strip (split and stack)
            unwarped = await self.image_manipulation_service.unwarp_tire_rim(
                resized_image, center_x, center_y, inner_radius, outer_radius
            )

            # 4) Transforming into slices
            processed_image = await self.image_manipulation_service.copy_and_append_image_portion_from_left(
                unwarped, 0.17
            )
            processed_image = await self.image_manipulation_service.slice_and_stack(
                processed_image, 2
            )

            # 5) Global properties enhancement
            processed_image = await self.image_manipulation_service.perform_clahe(
                processed_image
            )
            processed_image = (
                await self.image_manipulation_service.perform_bilateral_filter(
                    processed_image
                )
            )
            processed_image = await self.image_manipulation_service.perform_bitwise_not(
                processed_image
            )

            # 6) Emphasise characters via segmentation pipeline
            try:
                emphasised = await self.image_segmentation_service.emphasise_characters_on_text_regions(
                    processed_image
                )
            except Exception as ex:
                duration_ms = int((time.perf_counter() - start) * 1000)
                return PreprocessingResultDto(
                    status="acceptable_failure",
                    message=f"Character emphasisation failed: {str(ex)}",
                    image=emphasised,
                    duration_ms=duration_ms,
                )

            duration_ms = int((time.perf_counter() - start) * 1000)
            return PreprocessingResultDto(
                status="success",
                message="Preprocessing completed successfully",
                image=emphasised,
                duration_ms=duration_ms,
            )
        except Exception as ex:
            duration_ms = int((time.perf_counter() - start) * 1000)
            return PreprocessingResultDto(
                status="unexpected_error",
                message=str(ex),
                image=command.image,
                duration_ms=duration_ms,
            )

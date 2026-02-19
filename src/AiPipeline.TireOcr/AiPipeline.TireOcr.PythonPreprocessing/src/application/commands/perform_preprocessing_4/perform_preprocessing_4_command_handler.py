from fastapi.concurrency import run_in_threadpool
import numpy as np
import cv2
import math

from src.application.constants.preprocessing_constants import (
    MAX_IMAGE_SIDE,
    SLICE_HORIZONTAL_OVERLAP_RATIO,
    TIRE_INNER_RADIUS_RATIO,
    TIRE_OUTER_RADIUS_RATIO,
    TIRE_STRIP_PROLONG_WIDTH_RATIO,
)
from src.application.commands.perform_preprocessing_4.perform_preprocessing_4_command import (
    PerformPreprocessing4Command,
)
from src.application.services.image_slicer_service import ImageSlicerService
from ...services.image_manipulation_service import ImageManipulationService
from ...services.rim_detection_service import RimDetectionService
from ...services.image_segmentation_service import ImageSegmentationService
from ...dtos.preprocessing_result_dto import PreprocessingResultDto
import time


class PerformPreprocessing4CommandHandler:
    def __init__(
        self,
        image_manipulation_service: ImageManipulationService,
        rim_detection_service: RimDetectionService,
        image_segmentation_service: ImageSegmentationService,
        image_slicer_service: ImageSlicerService,
    ):
        self.image_manipulation_service = image_manipulation_service
        self.rim_detection_service = rim_detection_service
        self.image_segmentation_service = image_segmentation_service
        self.image_slicer_service = image_slicer_service

    async def handle(
        self, command: PerformPreprocessing4Command
    ) -> PreprocessingResultDto:
        start = time.perf_counter()
        try:
            nparr = np.frombuffer(command.image, np.uint8)
            color_image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
            if color_image is None:
                raise ValueError("Failed to decode input image")

            # 1) Resize to max dimension
            resized_image = self.image_manipulation_service.resize_to_max_dimension(
                color_image, MAX_IMAGE_SIDE
            )

            # 2) Rim detection (may throw if not found)
            try:
                center_x, center_y, radius = await run_in_threadpool(
                    self.rim_detection_service.detect_rim, resized_image
                )
            except Exception as ex:
                duration_ms = int((time.perf_counter() - start) * 1000)
                resized_bytes = self.image_manipulation_service.image_to_bytes(
                    resized_image
                )
                return PreprocessingResultDto(
                    status="acceptable_failure",
                    message=f"Rim detection failed: {str(ex)}",
                    image=resized_bytes,
                    duration_ms=duration_ms,
                )
            inner_radius = radius * TIRE_INNER_RADIUS_RATIO
            outer_radius = radius * TIRE_OUTER_RADIUS_RATIO

            # 3) Unwarp tire rim to strip (split and stack)
            unwarped_color = self.image_manipulation_service.unwarp_tire_rim(
                resized_image, center_x, center_y, inner_radius, outer_radius
            )

            # Convert to grayscale for the rest of the pipeline
            unwarped_gray = self.image_manipulation_service.ensure_grayscale(
                unwarped_color
            )

            # 4) Transforming into slices
            processed_image = (
                self.image_manipulation_service.copy_and_append_image_portion_from_left(
                    unwarped_gray, TIRE_STRIP_PROLONG_WIDTH_RATIO
                )
            )

            h, w = processed_image.shape
            slices = self.image_slicer_service.slice_image_with_additive_overlap(
                processed_image,
                (math.ceil(w / 2), h),
                SLICE_HORIZONTAL_OVERLAP_RATIO,
                0,
            )
            processed_image = self.image_slicer_service.stack_images_vertically(slices)

            # 5) Global properties enhancement
            processed_image = self.image_manipulation_service.perform_clahe(
                processed_image
            )
            processed_image = self.image_manipulation_service.perform_bilateral_filter(
                processed_image
            )
            processed_image = self.image_manipulation_service.perform_bitwise_not(
                processed_image
            )

            # 6) Emphasise characters via segmentation pipeline
            try:
                emphasised = await run_in_threadpool(
                    self.image_segmentation_service.emphasise_characters_on_text_regions,
                    processed_image,
                )
            except Exception as ex:
                duration_ms = int((time.perf_counter() - start) * 1000)
                # Encode last processed image to bytes for DTO
                processed_bytes = self.image_manipulation_service.image_to_bytes(
                    processed_image
                )
                return PreprocessingResultDto(
                    status="acceptable_failure",
                    message=f"Character emphasisation failed: {str(ex)}",
                    image=processed_bytes,
                    duration_ms=duration_ms,
                )

            # Encode emphasised image to bytes for DTO
            emphasised_bytes = self.image_manipulation_service.image_to_bytes(
                emphasised
            )

            duration_ms = int((time.perf_counter() - start) * 1000)
            return PreprocessingResultDto(
                status="success",
                message="Preprocessing completed successfully",
                image=emphasised_bytes,
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

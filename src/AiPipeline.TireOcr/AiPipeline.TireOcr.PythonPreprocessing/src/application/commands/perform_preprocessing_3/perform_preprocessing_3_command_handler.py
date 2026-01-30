from ..perform_preprocessing_3.perform_preprocessing_3_command import (
    PerformPreprocessing3Command,
)
from ...services.image_manipulation_service import ImageManipulationService
from ...services.rim_detection_service import RimDetectionService
from ...dtos.preprocessing_result_dto import PreprocessingResultDto


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

    def handle(self, command: PerformPreprocessing3Command) -> PreprocessingResultDto:
        # TODO: Implement preprocessing logic
        pass

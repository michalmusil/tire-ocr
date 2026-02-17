from src.application.commands.perform_preprocessing_3.perform_preprocessing_3_command_handler import (
    PerformPreprocessing3CommandHandler,
)
from src.application.commands.perform_preprocessing_4.perform_preprocessing_4_command_handler import (
    PerformPreprocessing4CommandHandler,
)
from src.application.services.image_manipulation_service import ImageManipulationService
from src.application.services.image_segmentation_service import ImageSegmentationService
from src.application.services.rim_detection_service import RimDetectionService
from src.infrastructure.services.image_manipulation_service_impl import (
    ImageManipulationServiceImpl,
)
from src.infrastructure.services.image_segmentation_service_impl import (
    ImageSegmentationServiceImpl,
)
from src.infrastructure.services.rim_detection_service_impl import (
    RimDetectionServiceImpl,
)

# Services
_image_manipulation_service_instance: ImageManipulationService = (
    ImageManipulationServiceImpl()
)
_image_segmentation_service_instance: ImageSegmentationService = (
    ImageSegmentationServiceImpl()
)
_rim_detection_service_instance: RimDetectionService = RimDetectionServiceImpl()


def get_image_manipulation_service() -> ImageManipulationService:
    return _image_manipulation_service_instance


def get_image_segmentation_service() -> ImageSegmentationService:
    return _image_segmentation_service_instance


def get_rim_detection_service() -> RimDetectionService:
    return _rim_detection_service_instance


# Commands\Queries
_perform_preprocessing_3_command_handler_instance: (
    PerformPreprocessing3CommandHandler
) = PerformPreprocessing3CommandHandler(
    _image_manipulation_service_instance,
    _rim_detection_service_instance,
    _image_segmentation_service_instance,
)

_perform_preprocessing_4_command_handler_instance: (
    PerformPreprocessing4CommandHandler
) = PerformPreprocessing4CommandHandler(
    _image_manipulation_service_instance,
    _rim_detection_service_instance,
    _image_segmentation_service_instance,
)


def get_perform_preprocessing_3_command_handler():
    return _perform_preprocessing_3_command_handler_instance


def get_perform_preprocessing_4_command_handler():
    return _perform_preprocessing_4_command_handler_instance

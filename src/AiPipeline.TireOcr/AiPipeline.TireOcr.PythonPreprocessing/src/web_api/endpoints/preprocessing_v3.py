from fastapi import APIRouter, File, UploadFile, HTTPException
from fastapi.responses import StreamingResponse, JSONResponse
from ...application.commands.perform_preprocessing_3.perform_preprocessing_3_command import (
    PerformPreprocessing3Command,
)
from ...application.commands.perform_preprocessing_3.perform_preprocessing_3_command_handler import (
    PerformPreprocessing3CommandHandler,
)
from ...infrastructure.services.image_manipulation_service_impl import (
    CvImageManipulationService,
)
from ...infrastructure.services.rim_detection_service_impl import (
    RimDetectionServiceImpl,
)
from ...infrastructure.services.image_segmentation_service_impl import (
    ImageSegmentationServiceImpl,
)

router = APIRouter(prefix="/preprocessing", tags=["Preprocessing"])


@router.post("/v3")
async def preprocessing_v3(image: UploadFile = File(...)):
    try:
        image_bytes = await image.read()
        command = PerformPreprocessing3Command(image=image_bytes)

        handler = PerformPreprocessing3CommandHandler(
            image_manipulation_service=CvImageManipulationService(),
            rim_detection_service=RimDetectionServiceImpl(),
            image_segmentation_service=ImageSegmentationServiceImpl(),
        )
        result = await handler.handle(command)

        if result.status == "error":
            return JSONResponse(
                status_code=400,
                content={
                    "status": result.status,
                    "message": result.message,
                    "duration_ms": result.duration_ms,
                },
            )

        return StreamingResponse(iter([result.image]), media_type="image/png")
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

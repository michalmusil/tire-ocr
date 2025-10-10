from pydantic import BaseModel, Field


class OCRResponse(BaseModel):
    detectedCode: str = Field(
        ..., description="Text detected in the image via Tire OCR"
    )
    durationMs: int = Field(..., description="Duration of Tire OCR task")

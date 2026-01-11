from typing import Literal, Union
from pydantic import BaseModel, Field


class OCRResult(BaseModel):
    status: Literal["success", "not_found", "error"] = Field(
        ..., description="Status of the OCR process."
    )
    message: str = Field(..., description="A human-readable message about the result.")
    extracted_text: Union[str, None] = (
        Field(None, description="The recognized text, if successful. Otherwise None"),
    )
    duration_ms: int = Field(..., description="Duration of Tire OCR task")

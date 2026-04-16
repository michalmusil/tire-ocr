from pydantic import BaseModel, Field
from typing import Literal


class PreprocessingResultDto(BaseModel):
    status: Literal["success", "acceptable_failure", "unexpected_error"] = Field(
        ..., description="Status of the preprocessing process."
    )
    message: str = Field(..., description="A human-readable message about the result.")
    image: bytes = Field(..., description="The preprocessed image as bytes.")
    duration_ms: int = Field(..., description="Duration of preprocessing task")

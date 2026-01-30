from pydantic import BaseModel, Field


class PerformPreprocessing3Command(BaseModel):
    image: bytes = Field(..., description="The image to preprocess as bytes.")

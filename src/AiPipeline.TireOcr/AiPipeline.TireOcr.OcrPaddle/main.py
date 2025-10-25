from fastapi.responses import RedirectResponse
import uvicorn
from fastapi import FastAPI

from src.web_api.endpoints.ocr import router as ocr_router

app: FastAPI = FastAPI(
    title="PaddleOCR FastAPI Service",
    description="A REST API for Optical Character Recognition using PaddleOCR.",
    version="1.0.0",
)
app.include_router(ocr_router)


@app.get("/", include_in_schema=False)
async def root():
    return RedirectResponse("/docs")


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=10100)

from fastapi.responses import RedirectResponse
import uvicorn
from fastapi import FastAPI


app: FastAPI = FastAPI(
    title="Tire OCR FastAPI Preprocessing Service",
    description="A REST API for Optical Character Recognition preprocessing.",
    version="1.0.0",
)
# app.include_router(ocr_router)


@app.get("/", include_in_schema=False)
async def root():
    return RedirectResponse("/docs")


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=10100)

import cv2
import numpy as np
from ...application.services.image_segmentation_service import ImageSegmentationService
from paddleocr import TextDetection
from typing import Optional, List, Tuple
import onnxruntime as ort
from imutils import perspective
import os

_TEXT_DETECTION_ENGINE: Optional[TextDetection] = None

_UNET_SESSION: Optional[ort.InferenceSession] = None
_UNET_INPUT_NAME: Optional[str] = None
_UNET_TARGET_W = 512
_UNET_TARGET_H = 128


def get_text_detection_engine() -> TextDetection:
    global _TEXT_DETECTION_ENGINE
    if _TEXT_DETECTION_ENGINE is None:
        print("TextDetection: INITIALIZING")
        _TEXT_DETECTION_ENGINE = TextDetection(model_name="PP-OCRv5_server_det")
        print("Paddle TextDetection: INITIALIZED")
    return _TEXT_DETECTION_ENGINE


def _get_unet_session() -> Tuple[ort.InferenceSession, str]:
    global _UNET_SESSION, _UNET_INPUT_NAME
    if _UNET_SESSION is not None:
        return _UNET_SESSION, _UNET_INPUT_NAME
    model_path = os.path.abspath(
        os.path.join(
            os.getcwd(),
            "models/unet_text_region/tire_unet.onnx",
        )
    )
    sess = ort.InferenceSession(model_path, providers=["CPUExecutionProvider"])
    _UNET_SESSION = sess
    _UNET_INPUT_NAME = sess.get_inputs()[0].name
    return _UNET_SESSION, _UNET_INPUT_NAME


class ImageSegmentationServiceImpl(ImageSegmentationService):
    async def emphasise_characters(self, image_bytes: bytes) -> bytes:
        np_arr = np.frombuffer(image_bytes, np.uint8)
        img = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)
        if img is None:
            return image_bytes

        detector = get_text_detection_engine()
        polys = self._parse_text_polygons(detector, img)
        if not polys:
            return image_bytes

        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        enhanced = self._init_enhanced_canvas(img)

        session, input_name = _get_unet_session()
        H, W = gray.shape[:2]

        # Build batch of warped crops and corresponding inverse homographies
        jobs: List[Tuple[np.ndarray, np.ndarray]] = []
        for poly in polys:
            pts = self._normalize_quad(poly)
            warped = self._warp_text_region(gray, pts)
            if warped.size == 0:
                continue
            Minv = self._compute_inverse_homography(pts, warped.shape)
            jobs.append((warped, Minv))

        if not jobs:
            return image_bytes

        # One batched ONNX run for all warped crops
        batch, metas = self._build_unet_batch(jobs)
        outputs = session.run(None, {input_name: batch})
        self._apply_masks_from_batch(outputs, metas, (W, H), gray, enhanced)

        _, out_bytes = cv2.imencode(".png", enhanced)
        return out_bytes.tobytes()

    # ---------- Helpers (readability & SRP) ----------
    def _parse_text_polygons(
        self, detector: TextDetection, img: np.ndarray
    ) -> List[np.ndarray]:
        try:
            result = detector.predict(img)
            polys: List[np.ndarray] = []
            for page in result:
                if "dt_polys" in page:
                    polys.extend(
                        [np.array(p, dtype=np.float32) for p in page["dt_polys"]]
                    )
            return polys
        except Exception:
            return []

    def _init_enhanced_canvas(self, img: np.ndarray) -> np.ndarray:
        enhanced = img.copy()
        enhanced = cv2.convertScaleAbs(enhanced, alpha=2.5, beta=0)
        return enhanced

    def _normalize_quad(self, poly: np.ndarray) -> np.ndarray:
        return poly.reshape(4, 2).astype("float32")

    def _warp_text_region(self, gray: np.ndarray, pts: np.ndarray) -> np.ndarray:
        return perspective.four_point_transform(gray, pts)

    def _compute_inverse_homography(
        self, pts: np.ndarray, warped_shape: Tuple[int, int]
    ) -> np.ndarray:
        h_warp, w_warp = warped_shape[:2]
        dst = np.array(
            [[0, 0], [w_warp - 1, 0], [w_warp - 1, h_warp - 1], [0, h_warp - 1]],
            dtype=np.float32,
        )
        ordered_pts = perspective.order_points(pts)
        return cv2.getPerspectiveTransform(dst, ordered_pts.astype(np.float32))

    def _inverse_warp_mask(
        self, bin_mask_warped: np.ndarray, Minv: np.ndarray, size_wh: Tuple[int, int]
    ) -> np.ndarray:
        W, H = size_wh
        return cv2.warpPerspective(
            bin_mask_warped, Minv, (W, H), flags=cv2.INTER_NEAREST
        )

    def _apply_dark_overlay(
        self,
        enhanced: np.ndarray,
        gray: np.ndarray,
        mask_full: np.ndarray,
        base_alpha: float = 0.6,
    ) -> None:
        visual_base = cv2.cvtColor(gray, cv2.COLOR_GRAY2BGR)
        black_layer = np.zeros_like(visual_base)
        overlay = cv2.addWeighted(
            visual_base, base_alpha, black_layer, 1.0 - base_alpha, 0
        )
        region = mask_full > 0
        enhanced[region] = overlay[region]

    def _build_unet_batch(
        self, jobs: List[Tuple[np.ndarray, np.ndarray]]
    ) -> Tuple[np.ndarray, List[Tuple[int, int, int, int, int, int, np.ndarray]]]:
        """Prepare a batched tensor for U-Net and collect metadata for inverse mapping.
        Returns (batch[N,1,H,W], metas per item: (nw,nh,dx,dy,w_warp,h_warp,Minv))."""
        blobs: List[np.ndarray] = []
        metas: List[Tuple[int, int, int, int, int, int, np.ndarray]] = []
        for warped, Minv in jobs:
            canvas, (nw, nh), (dx, dy) = self._letterbox_resize_gray(
                warped, _UNET_TARGET_W, _UNET_TARGET_H
            )
            blob = canvas.astype(np.float32) / 255.0
            blob = blob[np.newaxis, np.newaxis, :, :]
            blobs.append(blob)
            h_warp, w_warp = warped.shape[:2]
            metas.append((nw, nh, dx, dy, w_warp, h_warp, Minv))
        batch = np.concatenate(blobs, axis=0)
        return batch, metas

    def _apply_masks_from_batch(
        self,
        outputs: List[np.ndarray],
        metas: List[Tuple[int, int, int, int, int, int, np.ndarray]],
        size_wh: Tuple[int, int],
        gray: np.ndarray,
        enhanced: np.ndarray,
    ) -> None:
        """Post-process batched U-Net outputs and composite them back to the image."""
        out = outputs[0]  # shape [N,1,H,W]
        W, H = size_wh
        for i, meta in enumerate(metas):
            nw, nh, dx, dy, w_warp, h_warp, Minv = meta
            out_mask = out[i, 0]
            bin_mask = (out_mask > 0.25).astype(np.uint8) * 255
            # Remove letterbox padding
            cropped = bin_mask[dy : dy + nh, dx : dx + nw]
            # Resize back to warped size
            mask_warp = cv2.resize(
                cropped, (w_warp, h_warp), interpolation=cv2.INTER_NEAREST
            )
            # Inverse warp to original image space
            mask_full = cv2.warpPerspective(
                mask_warp, Minv, (W, H), flags=cv2.INTER_NEAREST
            )
            # Composite
            self._apply_dark_overlay(enhanced, gray, mask_full, base_alpha=0.6)

    def _letterbox_resize_gray(self, image: np.ndarray, target_w: int, target_h: int):
        ih, iw = image.shape[:2]
        scale = min(target_w / iw, target_h / ih)
        nw, nh = int(iw * scale), int(ih * scale)
        resized = cv2.resize(image, (nw, nh), interpolation=cv2.INTER_AREA)
        canvas = np.zeros((target_h, target_w), dtype=np.uint8)
        dx, dy = (target_w - nw) // 2, (target_h - nh) // 2
        canvas[dy : dy + nh, dx : dx + nw] = resized
        return canvas, (nw, nh), (dx, dy)

    # def _run_unet_letterbox_to_roi_mask(
    #     self, session: ort.InferenceSession, input_name: str, roi_gray: np.ndarray
    # ) -> np.ndarray:
    #     # Preprocess
    #     canvas, (nw, nh), (dx, dy) = self._letterbox_resize_gray(
    #         roi_gray, _UNET_TARGET_W, _UNET_TARGET_H
    #     )
    #     blob = canvas.astype(np.float32) / 255.0
    #     blob = blob[np.newaxis, np.newaxis, :, :]

    #     # Infer
    #     outputs = session.run(None, {input_name: blob})
    #     out_mask = outputs[0][0][0]  # HxW (target)

    #     # Binary mask
    #     bin_mask = (out_mask > 0.25).astype(np.uint8) * 255  # 0/255

    #     # Crop valid region (remove letterbox padding) back to model-resized ROI
    #     cropped = bin_mask[dy : dy + nh, dx : dx + nw]

    #     # Resize back to the original ROI size
    #     mask_resized = cv2.resize(
    #         cropped,
    #         (roi_gray.shape[1], roi_gray.shape[0]),
    #         interpolation=cv2.INTER_NEAREST,
    #     )
    #     return mask_resized

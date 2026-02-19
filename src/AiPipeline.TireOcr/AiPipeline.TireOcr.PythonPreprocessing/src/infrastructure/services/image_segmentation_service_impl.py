import cv2
import numpy as np
from ...application.services.image_segmentation_service import ImageSegmentationService
from ...application.services.image_manipulation_service import ImageManipulationService
from paddleocr import TextDetection
from typing import Optional, List, Tuple
import onnxruntime as ort
from imutils import perspective
import os
import rpack

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
    def emphasise_characters_on_text_regions(self, image: np.ndarray) -> np.ndarray:
        if image is None:
            raise ValueError("Input image is None")
        img = self._ensure_grayscale(image)

        detector = get_text_detection_engine()
        polys = self._parse_text_polygons(detector, img)
        if not polys:
            raise ValueError("No text polygons found")

        final = img.copy()
        blurred_img = cv2.GaussianBlur(img, (3, 3), 0)
        sobel_img = self._apply_sobel(blurred_img, erode=True)
        # Threshold the lower values of sobel image to remove ghosts
        _, sobel_img = cv2.threshold(sobel_img, 80, 255, cv2.THRESH_BINARY)

        session, input_name = _get_unet_session()
        H, W = img.shape[:2]

        for poly in polys:
            pts = self._normalize_quad(poly)
            warped = self._warp_text_region(img, pts)
            if warped.size == 0:
                continue

            bin_mask_warped = self._run_unet_letterbox_to_roi_mask(
                session, input_name, warped
            )
            Minv = self._compute_inverse_homography(pts, warped.shape)
            mask_full = self._inverse_warp_mask(bin_mask_warped, Minv, (W, H))
            # blur the mask to remove sharp edges
            mask_full = cv2.GaussianBlur(mask_full, (15, 15), 0)
            # convert mask to higher dimension
            mask_full = mask_full.astype(np.float32) / 255.0
            # apply the mask to the image
            final = self._blend_within_mask_region(sobel_img, final, mask_full)
        return final

    def compose_emphasised_text_region_mosaic(
        self, image: np.ndarray, emphasise_characters: bool
    ) -> np.ndarray:
        if image is None:
            raise ValueError("Input image is None")
        img = self._ensure_grayscale(image)

        detector = get_text_detection_engine()
        polys = self._parse_text_polygons(detector, img)
        if not polys:
            raise ValueError("No text polygons found")
        gray = img
        # Compute average intensity of the input image for mosaic background
        avg_intensity = int(np.mean(gray))

        session, input_name = _get_unet_session()

        enhanced_slices: List[np.ndarray] = []
        rects: List[Tuple[int, int]] = []

        for poly in polys:
            pts = self._normalize_quad(poly)
            warped = self._warp_text_region(gray, pts)

            if not emphasise_characters:
                enhanced_slices.append(warped)
                rects.append((warped.shape[1], warped.shape[0]))
                continue

            if warped.size == 0:
                continue

            bin_mask_warped = self._run_unet_letterbox_to_roi_mask(
                session, input_name, warped
            )

            # Apply Gaussian blur to warped image before Sobel (for edge detection only)
            warped_blur = cv2.GaussianBlur(warped, (3, 3), 0)
            sobel_edges = self._apply_sobel(warped_blur)

            # Blur the mask to soften edges and convert to soft mask in [0,1]
            mask_blur = cv2.GaussianBlur(bin_mask_warped, (15, 15), 0)
            soft_mask = mask_blur.astype(np.float32) / 255.0

            # Apply highlighted edges only in the area of mask:
            # result = edges * softMask + warped * (1 - softMask)
            blended = self._blend_within_mask_region(sobel_edges, warped, soft_mask)

            h, w = blended.shape[:2]
            enhanced_slices.append(blended)
            rects.append((w, h))

        if not enhanced_slices:
            raise ValueError("No valid text regions for mosaic")

        # Use rpack to pack the enhanced slices into a mosaic
        positions = rpack.pack(rects)

        # Determine overall mosaic size using rpack.bbox_size
        max_w, max_h = rpack.bbox_size(rects, positions)

        # Initialize mosaic with the average color of the input image
        mosaic = np.full((max_h, max_w), avg_intensity, dtype=np.uint8)
        for slice_img, (w, h), (x, y) in zip(enhanced_slices, rects, positions):
            mosaic[y : y + h, x : x + w] = slice_img

        return mosaic

    def _ensure_grayscale(self, image: np.ndarray) -> np.ndarray:
        if image.ndim == 3:
            return cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        return image

    # def remove_small_blobs(self, img, min_size=5):
    #     # Find all disconnected white patches
    #     nb_components, output, stats, centroids = cv2.connectedComponentsWithStats(
    #         img, connectivity=8
    #     )

    #     # Get the area of each component (index 4 in stats)
    #     sizes = stats[1:, -1]
    #     new_img = np.zeros((output.shape), dtype=np.uint8)

    #     for i in range(0, nb_components - 1):
    #         if sizes[i] >= min_size:
    #             new_img[output == i + 1] = 255

    #     return new_img

    def _apply_sobel(self, src, erode: bool = False):
        grad_x = cv2.Sobel(src, cv2.CV_16S, 1, 0, ksize=3)
        grad_y = cv2.Sobel(src, cv2.CV_16S, 0, 1, ksize=3)
        abs_x = cv2.convertScaleAbs(grad_x)
        abs_y = cv2.convertScaleAbs(grad_y)
        final = cv2.addWeighted(abs_x, 0.5, abs_y, 0.5, 0)
        if erode:
            final = cv2.erode(final, (3, 3), iterations=1)
        return final

    def _blend_within_mask_region(self, sobel_edges, source, mask):
        sobel_f = sobel_edges.astype(np.float32)
        source_f = source.astype(np.float32)

        sharpened_part = sobel_f * mask
        original_part = source_f * (1.0 - mask)
        blended = sharpened_part + original_part
        blended = np.clip(blended, 0, 255).astype(np.uint8)
        return blended

    def _parse_text_polygons(
        self, detector: TextDetection, img: np.ndarray
    ) -> List[np.ndarray]:
        try:
            colored = cv2.cvtColor(img, cv2.COLOR_GRAY2BGR)
            result = detector.predict(colored)
            polys: List[np.ndarray] = []
            for page in result:
                if "dt_polys" in page:
                    polys.extend(
                        [np.array(p, dtype=np.float32) for p in page["dt_polys"]]
                    )
            return polys
        except Exception as e:
            print(f"Error parsing text polygons: {e}")
            return []

    def _normalize_quad(self, poly: np.ndarray) -> np.ndarray:
        return poly.reshape(4, 2).astype("float32")

    def _warp_text_region(self, gray, pts):
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

    def _letterbox_resize_gray(self, image: np.ndarray, target_w: int, target_h: int):
        ih, iw = image.shape[:2]
        scale = min(target_w / iw, target_h / ih)
        nw, nh = int(iw * scale), int(ih * scale)
        resized = cv2.resize(image, (nw, nh), interpolation=cv2.INTER_AREA)
        canvas = np.zeros((target_h, target_w), dtype=np.uint8)
        dx, dy = (target_w - nw) // 2, (target_h - nh) // 2
        canvas[dy : dy + nh, dx : dx + nw] = resized
        return canvas, (nw, nh), (dx, dy)

    def _run_unet_letterbox_to_roi_mask(
        self, session: ort.InferenceSession, input_name: str, roi_gray: np.ndarray
    ) -> np.ndarray:
        # Preprocess
        canvas, (nw, nh), (dx, dy) = self._letterbox_resize_gray(
            roi_gray, _UNET_TARGET_W, _UNET_TARGET_H
        )
        blob = canvas.astype(np.float32) / 255.0
        blob = blob[np.newaxis, np.newaxis, :, :]

        # Infer
        outputs = session.run(None, {input_name: blob})
        out_mask = outputs[0][0][0]  # HxW (target)

        # Binary mask
        bin_mask = (out_mask > 0.25).astype(np.uint8) * 255  # 0/255

        # Crop valid region (remove letterbox padding) back to model-resized ROI
        cropped = bin_mask[dy : dy + nh, dx : dx + nw]

        # Resize back to the original ROI size
        mask_resized = cv2.resize(
            cropped,
            (roi_gray.shape[1], roi_gray.shape[0]),
            interpolation=cv2.INTER_NEAREST,
        )
        return mask_resized

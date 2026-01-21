import { z } from "zod";

export const PreprocessingTypeSchema = z.enum([
  "Resize",
  "ExtractRoi",
  "ExtractRoiAndRemoveBg",
  "ExtractAndComposeSlices",
]);
export type PreprocessingType = z.infer<typeof PreprocessingTypeSchema>;

export const OcrTypeSchema = z.enum([
  "GoogleGemini",
  "MistralPixtral",
  "OpenAiGpt",
  "GoogleCloudVision",
  "AzureAiVision",
  "PaddleOcr",
  "QwenVl",
  "InternVl",
  "EasyOcr",
  "DeepseekOcr",
]);
export type OcrType = z.infer<typeof OcrTypeSchema>;

export const PostprocessingTypeSchema = z.enum(["SimpleExtractValues"]);
export type PostprocessingType = z.infer<typeof PostprocessingTypeSchema>;

export const DbMatchingTypeSchema = z.enum(["None", "TireCodeAndManufacturer"]);
export type DbMatchingType = z.infer<typeof DbMatchingTypeSchema>;

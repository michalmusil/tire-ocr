import z from "zod";
import {
  DbMatchingTypeSchema,
  OcrTypeSchema,
  PostprocessingTypeSchema,
  PreprocessingTypeSchema,
} from "./run-steps-types";

export const RunConfigurationSchema = z.object({
  preprocessingType: PreprocessingTypeSchema,
  ocrType: OcrTypeSchema,
  postprocessingType: PostprocessingTypeSchema,
  dbMatchingType: DbMatchingTypeSchema,
});
export type RunConfiguration = z.infer<typeof RunConfigurationSchema>;

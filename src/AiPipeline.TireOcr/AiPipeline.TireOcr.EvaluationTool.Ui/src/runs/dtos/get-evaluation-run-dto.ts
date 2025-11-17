import { EvaluationResultCategorySchema } from "@/core/models/evaluation-result-category";
import { PaginationSchema } from "@/core/models/pagination";
import { RunConfigurationSchema } from "@/core/models/run-configuration";
import { z } from "zod";

const ImageFileSchema = z.object({
  fileName: z.string(),
  contentType: z.string(),
});

const TireCodeSchema = z.object({
  rawCode: z.string(),
  processedCode: z.string(),
  vehicleClass: z.string().nullish(),
  width: z.number().nullish(),
  aspectRatio: z.number().nullish(),
  construction: z.string().nullish(),
  diameter: z.number().nullish(),
  loadRange: z.string().nullish(),
  loadIndex: z.number().nullish(),
  loadIndex2: z.number().nullish(),
  speedRating: z.string().nullish(),
});

export type TireCode = z.infer<typeof TireCodeSchema>;

const EvaluationResultParameterSchema = z.object({
  distance: z.number(),
  estimatedAccuracy: z.number(),
});

const EstimatedCostsSchema = z.object({
  inputUnitCount: z.number().nullish(),
  outputUnitCount: z.number().nullish(),
  billingUnit: z.string().nullish(),
  estimatedCost: z.number().nullish(),
  estimatedCostCurrency: z.string().nullish(),
});

export const EvaluationRunSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string().nullish(),
  evaluationResultCategory: EvaluationResultCategorySchema,
  inputImage: ImageFileSchema,
  startedAt: z.iso.datetime(),
  finishedAt: z.iso.datetime().nullish(),
  runConfig: RunConfigurationSchema,
  failure: z
    .object({
      failureReason: z.string(),
      code: z.number(),
      message: z.string().nullish(),
    })
    .nullish(),
  evaluation: z
    .object({
      expectedTireCode: TireCodeSchema,
      totalDistance: z.number(),
      fullMatchParameterCount: z.number(),
      estimatedAccuracy: z.number(),
      vehicleClassEvaluation: EvaluationResultParameterSchema.nullish(),
      widthEvaluation: EvaluationResultParameterSchema.nullish(),
      diameterEvaluation: EvaluationResultParameterSchema.nullish(),
      aspectRatioEvaluation: EvaluationResultParameterSchema.nullish(),
      constructionEvaluation: EvaluationResultParameterSchema.nullish(),
      loadRangeEvaluation: EvaluationResultParameterSchema.nullish(),
      loadIndexEvaluation: EvaluationResultParameterSchema.nullish(),
      loadIndex2Evaluation: EvaluationResultParameterSchema.nullish(),
      speedRatingEvaluation: EvaluationResultParameterSchema.nullish(),
    })
    .nullish(),
  preprocessingResult: z
    .object({
      preprocessedImage: ImageFileSchema,
      durationMs: z.number(),
    })
    .nullish(),
  ocrResult: z
    .object({
      detectedCode: z.string(),
      detectedManufacturer: z.string().nullish(),
      estimatedCosts: EstimatedCostsSchema.nullish(),
      durationMs: z.number(),
    })
    .nullish(),
  postprocessingResult: z
    .object({
      postprocessedTireCode: TireCodeSchema,
      durationMs: z.number(),
    })
    .nullish(),
  totalExecutionDurationMs: z.number(),
});
export type EvaluationRun = z.infer<typeof EvaluationRunSchema>;

export const PaginatedEvaluationRunsSchema = z.object({
  items: z.array(EvaluationRunSchema),
  pagination: PaginationSchema,
});
export type PaginatedEvaluationRuns = z.infer<
  typeof PaginatedEvaluationRunsSchema
>;

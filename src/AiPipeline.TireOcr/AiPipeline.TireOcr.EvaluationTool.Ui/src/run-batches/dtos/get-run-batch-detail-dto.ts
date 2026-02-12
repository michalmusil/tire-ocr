import { z } from "zod";
import { EvaluationRunSchema } from "@/runs/dtos/get-evaluation-run-dto";
import { RunConfigurationSchema } from "@/core/models/run-configuration";

const BatchEvaluationCountsSchema = z.object({
  totalCount: z.number(),
  fullyCorrectCount: z.number(),
  correctMainParametersCount: z.number(),
  insufficientExtractionCount: z.number(),
  falsePositiveCount: z.number(),
  failedPreprocessingCount: z.number(),
  failedOcrCount: z.number(),
  failedPostprocessingCount: z.number(),
  failedUnexpectedCount: z.number(),
});

const BatchEvaluationDistancesSchema = z.object({
  averageDistance: z.number(),
  averageVehicleClassDistance: z.number(),
  averageWidthDistance: z.number(),
  averageDiameterDistance: z.number(),
  averageAspectRatioDistance: z.number(),
  averageConstructionDistance: z.number(),
  averageLoadRangeDistance: z.number(),
  averageLoadIndexDistance: z.number(),
  averageLoadIndex2Distance: z.number(),
  averageSpeedRatingDistance: z.number(),
});

const BatchEvaluationStatisticsSchema = z.object({
  parameterSuccessRate: z.number(),
  falsePositiveRate: z.number(),
  averageCer: z.number(),
  averageInferenceCost: z.number(),
  averageLatencyMs: z.number(),
});

const BatchEvaluationSchema = z.object({
  statistics: BatchEvaluationStatisticsSchema,
  counts: BatchEvaluationCountsSchema,
  distances: BatchEvaluationDistancesSchema,
});

export type BatchEvaluation = z.infer<typeof BatchEvaluationSchema>;

export const RunBatchDetailSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string().nullish(),
  startedAt: z.iso.datetime().nullish(),
  finishedAt: z.iso.datetime().nullish(),
  batchConfig: RunConfigurationSchema.nullish(),
  batchEvaluation: BatchEvaluationSchema,
  evaluationRuns: z.array(EvaluationRunSchema),
});

export type RunBatchDetail = z.infer<typeof RunBatchDetailSchema>;

export const RunBatchDetailResponseSchema = z.object({
  item: RunBatchDetailSchema,
});

export type RunBatchDetailResponse = z.infer<
  typeof RunBatchDetailResponseSchema
>;

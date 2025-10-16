import { PaginationSchema } from "@/core/models/pagination";
import { z } from "zod";

export const RunBatchSchema = z.object({
  id: z.string(),
  title: z.string(),
  numberOfEvaluations: z.number(),
  createdAt: z.iso.datetime(),
  startedAt: z.iso.datetime().nullish(),
  finishedAt: z.iso.datetime().nullish(),
});

export type RunBatch = z.infer<typeof RunBatchSchema>;

export const PaginatedRunBatchesSchema = z.object({
  items: z.array(RunBatchSchema),
  pagination: PaginationSchema,
});

export type PaginatedRunBatches = z.infer<typeof PaginatedRunBatchesSchema>;

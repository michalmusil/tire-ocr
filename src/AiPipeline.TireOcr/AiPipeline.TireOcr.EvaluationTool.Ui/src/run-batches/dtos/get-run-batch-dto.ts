import { z } from "zod";

export const RunBatchSchema = z.object({
  id: z.string(),
  title: z.string(),
});

export type RunBatch = z.infer<typeof RunBatchSchema>;

export const PaginatedRunBatchesSchema = z.object({
  items: z.array(RunBatchSchema),
  pagination: z.object({
    pageNumber: z.number(),
    pageSize: z.number(),
    totalPages: z.number(),
    totalCount: z.number(),
  }),
});

export type PaginatedRunBatches = z.infer<typeof PaginatedRunBatchesSchema>;

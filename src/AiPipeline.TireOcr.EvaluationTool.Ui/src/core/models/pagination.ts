import { z } from "zod";

export const PaginationSchema = z.object({
  pageNumber: z.number(),
  pageSize: z.number(),
  totalPages: z.number(),
  totalCount: z.number(),
});

export type Pagination = z.infer<typeof PaginationSchema>;

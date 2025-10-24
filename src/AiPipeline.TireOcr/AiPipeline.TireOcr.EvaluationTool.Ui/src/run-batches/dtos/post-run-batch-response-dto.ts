import z from "zod";

export const PostRunBatchResponseSchema = z.object({
  result: z.object({
    id: z.string(),
    title: z.string(),
  }),
});
export type PostRunBatchResponse = z.infer<typeof PostRunBatchResponseSchema>;

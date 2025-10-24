import z from "zod";

export const PostRunResponseSchema = z.object({
  result: z.object({
    id: z.string(),
    title: z.string(),
  }),
});
export type PostRunResponse = z.infer<typeof PostRunResponseSchema>;

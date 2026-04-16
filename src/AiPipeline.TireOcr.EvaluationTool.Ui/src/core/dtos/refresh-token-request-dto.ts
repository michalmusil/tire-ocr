import z from "zod";

export const RefreshTokenRequestSchema = z.object({
  accessToken: z.string(),
  refreshToken: z.string(),
});

export type RefreshTokenRequest = z.infer<typeof RefreshTokenRequestSchema>;

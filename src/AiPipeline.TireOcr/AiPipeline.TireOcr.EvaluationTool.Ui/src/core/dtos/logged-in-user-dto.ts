import z from "zod";

export const AccessRefreshTokenPairSchema = z.object({
  accessToken: z.string(),
  refreshToken: z.string(),
  refreshExpiration: z.iso.datetime(),
});

export type AccessRefreshTokenPair = z.infer<
  typeof AccessRefreshTokenPairSchema
>;

export const LoggedInUserSchema = z.object({
  id: z.string(),
  username: z.string(),
  accessRefreshTokenPair: AccessRefreshTokenPairSchema,
});

export type LoggedInUser = z.infer<typeof LoggedInUserSchema>;

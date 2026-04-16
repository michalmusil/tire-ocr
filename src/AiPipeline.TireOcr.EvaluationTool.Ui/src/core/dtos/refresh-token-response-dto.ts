import z from "zod";
import { LoggedInUserSchema } from "./logged-in-user-dto";

export const RefreshTokenResponseSchema = z.object({
  loggedInUser: LoggedInUserSchema,
});

export type RefreshTokenResponse = z.infer<typeof RefreshTokenResponseSchema>;

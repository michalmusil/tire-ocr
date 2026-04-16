import z from "zod";
import { LoggedInUserSchema } from "./logged-in-user-dto";

export const LoginResponseSchema = z.object({
  loggedInUser: LoggedInUserSchema,
});

export type LoginResponse = z.infer<typeof LoginResponseSchema>;

import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import type { LoginRequest } from "../dtos/login-request-dto";
import { LoginResponseSchema } from "../dtos/login-response-dto";

const logIn = async (data: LoginRequest) => {
  const response = await axios.post("/api/v1/Auth/Login", data);
  if (response.status !== 200) {
    if (response.status === 401)
      throw new Error("Username or password are incorrect");

    throw new Error("Failed to log in");
  }
  const parsed = LoginResponseSchema.parse(response.data);
  return parsed.loggedInUser;
};

export const useLogInMutation = () => useMutation({ mutationFn: logIn });

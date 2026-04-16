import type React from "react";
import { useEffect } from "react";
import axios from "axios";
import type { AuthState } from "./use-auth-reducer";
import type { LoggedInUser } from "../dtos/logged-in-user-dto";
import { RefreshTokenResponseSchema } from "../dtos/refresh-token-response-dto";

type UseUnauthorizedInterceptorProps = {
  authState: AuthState | null;
  login: (loginResponse: LoggedInUser) => void;
  logout: () => void;
  dependencies: React.DependencyList;
};

export const useUnauthorizedInterceptor = ({
  authState,
  login,
  logout,
  dependencies,
}: UseUnauthorizedInterceptorProps) => {
  useEffect(() => {
    const interceptor = axios.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;

        // If error is 401 and we haven't retried yet
        if (
          error.response?.status === 401 &&
          !originalRequest._retry &&
          authState
        ) {
          originalRequest._retry = true;

          try {
            const refreshedUser = await refreshToken(
              authState.accessToken,
              authState.refreshToken
            );
            if (!refreshedUser) throw new Error("Refreshing token failed");
            login(refreshedUser);

            // Retry the original request with new token
            originalRequest.headers[
              "Authorization"
            ] = `Bearer ${refreshedUser.accessRefreshTokenPair.accessToken}`;
            return axios(originalRequest);
          } catch (refreshError) {
            // If refresh fails, log the user out
            logout();
            return Promise.reject(refreshError);
          }
        }

        return Promise.reject(error);
      }
    );

    // Interceptor should be cleaned up on unmounting
    return () => {
      axios.interceptors.response.eject(interceptor);
    };
  }, dependencies);
};

const refreshToken = async (
  accessToken: string,
  refreshToken: string
): Promise<LoggedInUser | null> => {
  try {
    const response = await axios.post("/api/v1/Auth/Refresh", {
      accessToken: accessToken,
      refreshToken: refreshToken,
    });
    if (response.status !== 200) {
      throw new Error("Failed to refresh token");
    }
    const newUser = RefreshTokenResponseSchema.parse(response.data);

    return newUser.loggedInUser;
  } catch (error) {
    return null;
  }
};

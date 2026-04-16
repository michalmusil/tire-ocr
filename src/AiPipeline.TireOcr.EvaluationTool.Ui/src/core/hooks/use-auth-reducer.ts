import axios from "axios";
import {
  LoggedInUserSchema,
  type LoggedInUser,
} from "../dtos/logged-in-user-dto";
import { useReducer } from "react";

const AT_KEY = "accessToken";
const RT_KEY = "refreshToken";
const USER_KEY = "user";

export type AuthState = {
  user: LoggedInUser;
  accessToken: string;
  refreshToken: string;
};

type AuthAction =
  | { type: "setAuth"; user: LoggedInUser }
  | { type: "clearAuth" };

export const useAuthReducer = () => {
  const [authState, dispatchAuthState] = useReducer(
    authReducer,
    getInitialState()
  );

  const login = (user: LoggedInUser) => {
    dispatchAuthState({ type: "setAuth", user: user });
  };

  const logout = () => {
    dispatchAuthState({ type: "clearAuth" });
  };

  return {
    authState,
    login,
    logout,
  };
};

const authReducer = (
  state: AuthState | null,
  action: AuthAction
): AuthState | null => {
  switch (action.type) {
    case "setAuth": {
      const loggedInUser = action.user;
      const { accessToken, refreshToken } = loggedInUser.accessRefreshTokenPair;

      axios.defaults.headers.common["Authorization"] = `Bearer ${accessToken}`;
      // TODO: Storing tokens in local storage is NOT SECURE and should be refactored in case of production deployment
      localStorage.setItem(AT_KEY, accessToken);
      localStorage.setItem(RT_KEY, refreshToken);
      localStorage.setItem(USER_KEY, JSON.stringify(loggedInUser));

      return {
        user: loggedInUser,
        accessToken,
        refreshToken,
      };
    }

    case "clearAuth": {
      delete axios.defaults.headers.common["Authorization"];
      localStorage.removeItem(AT_KEY);
      localStorage.removeItem(RT_KEY);
      localStorage.removeItem(USER_KEY);

      return null;
    }

    default:
      return state;
  }
};

const getInitialState = (): AuthState | null => {
  const accessToken = localStorage.getItem(AT_KEY);
  const refreshToken = localStorage.getItem(RT_KEY);
  const userStr = localStorage.getItem(USER_KEY);
  const user = userStr ? LoggedInUserSchema.parse(JSON.parse(userStr)) : null;

  if (!accessToken || !refreshToken || !user) {
    delete axios.defaults.headers.common["Authorization"];
    localStorage.removeItem(AT_KEY);
    localStorage.removeItem(RT_KEY);
    localStorage.removeItem(USER_KEY);
    return null;
  }

  if (accessToken) {
    axios.defaults.headers.common["Authorization"] = `Bearer ${accessToken}`;
  }

  return {
    user,
    accessToken,
    refreshToken,
  };
};

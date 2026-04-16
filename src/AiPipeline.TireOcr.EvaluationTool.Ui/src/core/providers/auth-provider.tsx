import { createContext, useContext, useMemo } from "react";
import type { LoggedInUser } from "../dtos/logged-in-user-dto";
import { useAuthReducer, type AuthState } from "../hooks/use-auth-reducer";
import { useUnauthorizedInterceptor } from "../hooks/use-unauthorized-interceptor";

type AuthContextType = {
  authState: AuthState | null;
  login: (loginResponse: LoggedInUser) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

type AuthProviderProps = {
  children: React.ReactNode;
};

const AuthProvider = ({ children }: AuthProviderProps) => {
  const { authState, login, logout } = useAuthReducer();

  useUnauthorizedInterceptor({
    authState,
    login,
    logout,
    dependencies: [authState],
  });

  const contextValue = useMemo(
    () => ({
      authState,
      login,
      logout,
    }),
    [authState]
  );

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

export default AuthProvider;

import { Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../providers/auth-provider";
import LoginForm from "../components/login-form";
import { useLogInMutation } from "../mutations/log-in-mutation";
import type { LoginRequest } from "../dtos/login-request-dto";

const LoginPage: React.FC = () => {
  const { authState, login } = useAuth();
  if (authState) {
    return <Navigate to="/" replace />;
  }

  const navigate = useNavigate();
  const logInMutation = useLogInMutation();

  const onSubmit = (data: LoginRequest) => {
    logInMutation.mutate(data, {
      onSuccess: (user) => {
        login(user);
        navigate(`/`);
      },
      onError: (error) => {
        console.log(error);
      },
    });
  };

  return (
    <div className="h-screen flex items-center justify-center">
      <LoginForm
        onSubmit={onSubmit}
        isSubmitting={logInMutation.isPending}
        error={logInMutation.error?.message}
      />
    </div>
  );
};

export default LoginPage;

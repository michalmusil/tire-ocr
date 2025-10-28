import { FormProvider, useForm } from "react-hook-form";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "./ui/card";
import FormInput from "./form-input";
import { Button } from "./ui/button";
import z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { type LoginRequest } from "../dtos/login-request-dto";
import { Spinner } from "./ui/spinner";
import { ModeToggle } from "./mode-toggle";

type LoginFormProps = {
  onSubmit: (data: LoginRequest) => void;
  isSubmitting: boolean;
  error?: string;
};

const formSchema = z.object({
  username: z.string().min(1, "Username is required"),
  password: z.string().min(1, "Password is required"),
});
type LoginFormSchema = z.infer<typeof formSchema>;

const LoginForm = ({ onSubmit, isSubmitting, error }: LoginFormProps) => {
  const form = useForm<LoginFormSchema>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  return (
    <Card className="min-w-md relative">
      <div className="absolute top-6 right-4">
        <ModeToggle />
      </div>
      <CardHeader className="flex flex-col items-center gap-1">
        <CardTitle className="text-2xl font-bold text-center">
          Tire OCR Evaluation Tool
        </CardTitle>
        <CardDescription className="text-center">
          Sign in to access the evaluation platform
        </CardDescription>
      </CardHeader>
      <CardContent>
        <FormProvider {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="w-full flex flex-col items-center gap-2"
          >
            <FormInput<LoginFormSchema>
              label=""
              name="username"
              placeholder="Username"
            />
            <FormInput<LoginFormSchema>
              label=""
              name="password"
              type="password"
              placeholder="Password"
            />
            {error && (
              <div className="mt-2 text-sm text-red-600 dark:text-red-400">
                {error}
              </div>
            )}
            <Button
              type="submit"
              disabled={isSubmitting}
              className="mt-3 w-[12rem]"
            >
              {isSubmitting && <Spinner />}
              {isSubmitting ? "Logging in..." : "Log in"}
            </Button>
          </form>
        </FormProvider>
      </CardContent>
    </Card>
  );
};

export default LoginForm;

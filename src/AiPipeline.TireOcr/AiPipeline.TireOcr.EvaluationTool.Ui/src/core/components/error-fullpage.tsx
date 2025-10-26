import { AlertCircle } from "lucide-react";
import { cn } from "../lib/utils";
import { Card } from "./ui/card";

type ErrorFullpageProps = {
  errorMessage: string;
} & React.HTMLAttributes<HTMLDivElement>;

const ErrorFullpage = ({
  errorMessage,
  className,
  ...rest
}: ErrorFullpageProps) => {
  return (
    <div
      {...rest}
      className={cn(
        "flex items-center justify-center h-[calc(100vh-12rem)]",
        className
      )}
    >
      <Card className="p-14 bg-accent/25">
        <div className="flex flex-col items-center gap-y-4">
          <AlertCircle className="w-12 h-12 text-red-600 dark:text-red-400" />
          <p className="text-red-600 dark:text-red-400 text-lg font-semibold">
            {errorMessage}
          </p>
        </div>
      </Card>
    </div>
  );
};

export default ErrorFullpage;

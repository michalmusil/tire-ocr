import { Card } from "@/core/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogTrigger,
} from "@/core/components/ui/dialog";
import { Image, ZoomIn } from "lucide-react";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { cn } from "@/core/lib/utils";

type RunInputImageProps = {
  inputImage: EvaluationRun["inputImage"];
} & React.HTMLAttributes<HTMLDivElement>;

const RunInputImage = ({
  inputImage,
  className,
  ...rest
}: RunInputImageProps) => {
  const source = inputImage.fileName;
  const isUrl = source.startsWith("http://") || source.startsWith("https://");
  if (!isUrl) {
    return (
      <Card
        {...rest}
        className={cn(
          "flex flex-col items-center justify-center p-2",
          className
        )}
      >
        <Image className="h-8 w-8" />
        <p className="text-center">
          Preview only available for runs sourcing input image from URL
        </p>
      </Card>
    );
  }

  return (
    <Dialog>
      <DialogTrigger asChild>
        <div
          {...rest}
          className={cn(
            "relative group cursor-pointer self-start rounded-xl shadow-sm overflow-hidden",
            className
          )}
        >
          <img src={source} loading="lazy" alt="Input image" />
          <Overlay />
          <ZoomButton />
        </div>
      </DialogTrigger>
      <DialogContent
        showCloseButton={false}
        className="max-w-[80vw] max-h-[80vh] p-0"
      >
        <div className="w-full h-full flex items-center justify-center bg-background">
          <img
            src={source}
            alt="Input image enlarged"
            className="max-w-full max-h-full object-contain"
          />
        </div>
      </DialogContent>
    </Dialog>
  );
};

const Overlay = () => {
  return (
    <div className="absolute inset-0 bg-primary/0 group-hover:bg-primary/20 transition-colors duration-200" />
  );
};

const ZoomButton = () => {
  return (
    <div className="pointer-events-none absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-200">
      <div className="flex items-center gap-2 rounded-full bg-primary text-primary-foreground px-3 py-2 shadow-md">
        <ZoomIn className="h-5 w-5" />
        <span className="text-sm font-medium">Zoom</span>
      </div>
    </div>
  );
};

export default RunInputImage;

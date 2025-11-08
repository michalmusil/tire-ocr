import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { EvaluationRun, TireCode } from "../dtos/get-evaluation-run-dto";

type RunResultsCardProps = {
  run: EvaluationRun;
};

export const RunResultsCard = ({ run }: RunResultsCardProps) => {
  const expectedCode = run.evaluation?.expectedTireCode;
  const detectedCode = run.ocrResult?.detectedCode;
  const postprocessedCode = run.postprocessingResult?.postprocessedTireCode;
  const detectedManufacturer = run.ocrResult?.detectedManufacturer;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Results & Comparison</CardTitle>
        <CardDescription>
          Comparison of detected, postprocessed, and expected tire codes
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-y-6">
        {detectedCode && (
          <div className="flex flex-col gap-y-2">
            <p className="text-lg font-semibold text-muted-foreground">
              Raw OCR Detected Code
            </p>
            <p className="text-2xl font-mono font-bold">{detectedCode}</p>
            {detectedManufacturer && (
              <p className="text-sm text-muted-foreground">
                Manufacturer: {detectedManufacturer}
              </p>
            )}
          </div>
        )}

        {postprocessedCode && expectedCode && (
          <>
            <Separator />
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <TireCodeDisplay
                title="Postprocessed Code"
                tireCode={postprocessedCode}
                highlight={true}
              />
              <TireCodeDisplay
                title="Expected Code"
                tireCode={expectedCode}
                highlight={false}
              />
            </div>
          </>
        )}

        {run.failure && (
          <>
            <Separator />
            <div className="flex flex-col gap-y-2 p-4 rounded-lg bg-red-50 dark:bg-red-950">
              <p className="text-lg font-semibold text-destructive">
                Run Failed
              </p>
              <p className="text-sm font-medium">
                Reason: {run.failure.failureReason}
              </p>
              {run.failure.message && (
                <p className="text-sm text-muted-foreground">
                  {run.failure.message}
                </p>
              )}
              <p className="text-xs text-muted-foreground">
                Error Code: {run.failure.code}
              </p>
            </div>
          </>
        )}
      </CardContent>
    </Card>
  );
};

const TireCodeDisplay = ({
  title,
  tireCode,
  highlight,
}: {
  title: string;
  tireCode: TireCode;
  highlight: boolean;
}) => {
  return (
    <div
      className={`flex flex-col gap-y-2 p-4 rounded-lg border-2 ${
        highlight
          ? "border-blue-500 bg-blue-50 dark:bg-blue-950"
          : "border-border dark:border-border-dark"
      }`}
    >
      <p className="text-md font-semibold text-muted-foreground">{title}</p>
      <CodeItem label="Full Code" value={tireCode.processedCode} />
      <Separator />
      <div className="grid grid-cols-2 gap-2 text-sm">
        {tireCode.vehicleClass && (
          <CodeItem label="Vehicle Class" value={tireCode.vehicleClass} />
        )}
        {tireCode.width && (
          <CodeItem label="Width" value={tireCode.width.toString()} />
        )}
        {tireCode.diameter && (
          <CodeItem label="Diameter" value={tireCode.diameter.toString()} />
        )}
        {tireCode.aspectRatio && (
          <CodeItem
            label="Aspect Ratio"
            value={tireCode.aspectRatio.toString()}
          />
        )}
        {tireCode.construction && (
          <CodeItem label="Construction" value={tireCode.construction} />
        )}
        {tireCode.loadRange && (
          <CodeItem label="Load Range" value={tireCode.loadRange} />
        )}
        {tireCode.loadIndex && (
          <CodeItem label="Load Index" value={tireCode.loadIndex.toString()} />
        )}
        {tireCode.loadIndex2 && (
          <CodeItem
            label="Load Index 2"
            value={tireCode.loadIndex2.toString()}
          />
        )}
        {tireCode.speedRating && (
          <CodeItem label="Speed Rating" value={tireCode.speedRating} />
        )}
      </div>
    </div>
  );
};

const CodeItem = ({ label, value }: { label: string; value: string }) => {
  return (
    <div>
      <p className="text-xs text-muted-foreground">{label}</p>
      <p className={`text-sm font-semibold font-mono`}>{value}</p>
    </div>
  );
};

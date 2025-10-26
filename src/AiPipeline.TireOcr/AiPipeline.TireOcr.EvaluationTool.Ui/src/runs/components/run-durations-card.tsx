import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";
import { getDisplayedDurationFromMs } from "@/run-batches/utils/data-utils";

type RunDurationsCardProps = {
  run: EvaluationRun;
};

export const RunDurationsCard = ({ run }: RunDurationsCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Execution Durations</CardTitle>
        <CardDescription>Time taken by each step</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          <TimeMetric
            label="Total Duration"
            value={getDisplayedDurationFromMs(run.totalExecutionDurationMs)}
            highlight={true}
          />
          <Separator />
          {run.preprocessingResult && (
            <TimeMetric
              label="Preprocessing"
              value={getDisplayedDurationFromMs(
                run.preprocessingResult.durationMs
              )}
            />
          )}
          {run.ocrResult && (
            <TimeMetric
              label="OCR"
              value={getDisplayedDurationFromMs(run.ocrResult.durationMs)}
            />
          )}
          {run.postprocessingResult && (
            <TimeMetric
              label="Postprocessing"
              value={getDisplayedDurationFromMs(
                run.postprocessingResult.durationMs
              )}
            />
          )}
        </div>
      </CardContent>
    </Card>
  );
};

const TimeMetric = ({
  label,
  value,
  highlight = false,
}: {
  label: string;
  value: string;
  highlight?: boolean;
}) => {
  return (
    <div className="flex items-center justify-between">
      <p
        className={`${
          highlight ? "text-base" : "text-sm"
        } font-medium text-muted-foreground`}
      >
        {label}
      </p>
      <p className={`${highlight ? "text-2xl" : "text-md"} font-semibold`}>
        {value}
      </p>
    </div>
  );
};

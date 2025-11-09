import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { RunBatchDetail } from "../dtos/get-run-batch-detail-dto";
import { getDisplayedDurationFromMs } from "../utils/data-utils";

type BatchAverageTimesCardProps = {
  batchDetail: RunBatchDetail;
};

export const BatchAverageTimesCard = ({
  batchDetail,
}: BatchAverageTimesCardProps) => {
  const preprocessingRuns = batchDetail.evaluationRuns.filter(
    (run) => run.preprocessingResult?.durationMs
  );
  const averagePreprocessingDuration =
    preprocessingRuns.reduce(
      (acc, run) => acc + (run.preprocessingResult?.durationMs ?? 0),
      0
    ) / preprocessingRuns.length;

  const ocrRuns = batchDetail.evaluationRuns.filter(
    (run) => run.ocrResult?.durationMs
  );
  const averageOcrDuration =
    ocrRuns.reduce((acc, run) => acc + (run.ocrResult?.durationMs ?? 0), 0) /
    ocrRuns.length;

  const postprocessingRuns = batchDetail.evaluationRuns.filter(
    (run) => run.postprocessingResult?.durationMs
  );
  const averagePostprocessingDuration =
    postprocessingRuns.reduce(
      (acc, run) => acc + (run.postprocessingResult?.durationMs ?? 0),
      0
    ) / postprocessingRuns.length;

  const averageTotalDuration =
    batchDetail.evaluationRuns.reduce(
      (acc, run) => acc + (run.totalExecutionDurationMs ?? 0),
      0
    ) / batchDetail.evaluationRuns.length;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Average Durations</CardTitle>
        <CardDescription>
          Average durations of individual run steps per each run in the batch
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-6">
          <div className="space-y-2">
            <p className="text-lg font-semibold text-muted-foreground">
              Total Average
            </p>
            <p className="text-5xl font-bold">
              {getDisplayedDurationFromMs(averageTotalDuration)}
            </p>
          </div>
          <Separator />

          <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
            <TimeMetric
              label="Preprocessing average"
              value={getDisplayedDurationFromMs(averagePreprocessingDuration)}
            />
            <TimeMetric
              label="Ocr average"
              value={getDisplayedDurationFromMs(averageOcrDuration)}
            />

            <TimeMetric
              label="Postprocessing average"
              value={getDisplayedDurationFromMs(averagePostprocessingDuration)}
            />
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

const TimeMetric = ({ label, value }: { label: string; value: string }) => {
  return (
    <div className="flex flex-col items-start gap-y-1">
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      <p className="text-lg">{value}</p>
    </div>
  );
};

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { Separator } from "@/core/components/ui/separator";
import type { EvaluationRun } from "../dtos/get-evaluation-run-dto";

type RunEvaluationCardProps = {
  evaluation: NonNullable<EvaluationRun["evaluation"]>;
};

export const RunEvaluationCard = ({ evaluation }: RunEvaluationCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Evaluation Metrics</CardTitle>
        <CardDescription>
          Distance and accuracy metrics for tire code parameters
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex flex-col gap-y-6">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <MetricItem
              label="Total Distance"
              value={evaluation.totalDistance.toFixed(0)}
            />
            <MetricItem label="CER" value={evaluation.totalCer.toFixed(2)} />
            <MetricItem
              label="Full Match Count"
              value={evaluation.fullMatchParameterCount.toString()}
            />
            <MetricItem
              label="Estimated Accuracy"
              value={`${(evaluation.estimatedAccuracy * 100).toFixed(1)}%`}
            />
          </div>

          <Separator />

          <div>
            <p className="text-lg font-semibold text-muted-foreground mb-4">
              Parameter Evaluations
            </p>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {evaluation.vehicleClassEvaluation && (
                <ParameterEvaluation
                  label="Vehicle Class"
                  evaluation={evaluation.vehicleClassEvaluation}
                />
              )}
              {evaluation.widthEvaluation && (
                <ParameterEvaluation
                  label="Width"
                  evaluation={evaluation.widthEvaluation}
                />
              )}
              {evaluation.diameterEvaluation && (
                <ParameterEvaluation
                  label="Diameter"
                  evaluation={evaluation.diameterEvaluation}
                />
              )}
              {evaluation.aspectRatioEvaluation && (
                <ParameterEvaluation
                  label="Aspect Ratio"
                  evaluation={evaluation.aspectRatioEvaluation}
                />
              )}
              {evaluation.constructionEvaluation && (
                <ParameterEvaluation
                  label="Construction"
                  evaluation={evaluation.constructionEvaluation}
                />
              )}
              {evaluation.loadRangeEvaluation && (
                <ParameterEvaluation
                  label="Load Range"
                  evaluation={evaluation.loadRangeEvaluation}
                />
              )}
              {evaluation.loadIndexEvaluation && (
                <ParameterEvaluation
                  label="Load Index"
                  evaluation={evaluation.loadIndexEvaluation}
                />
              )}
              {evaluation.loadIndex2Evaluation && (
                <ParameterEvaluation
                  label="Load Index 2"
                  evaluation={evaluation.loadIndex2Evaluation}
                />
              )}
              {evaluation.speedRatingEvaluation && (
                <ParameterEvaluation
                  label="Speed Rating"
                  evaluation={evaluation.speedRatingEvaluation}
                />
              )}
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

const MetricItem = ({ label, value }: { label: string; value: string }) => {
  return (
    <div>
      <p className="text-sm font-medium text-muted-foreground">{label}</p>
      <p className="text-2xl font-bold">{value}</p>
    </div>
  );
};

const ParameterEvaluation = ({
  label,
  evaluation,
}: {
  label: string;
  evaluation: { distance: number; estimatedAccuracy: number };
}) => {
  const isExactMatch = evaluation.distance === 0;
  return (
    <div className="border rounded-lg p-3 space-y-1">
      <p className="text-sm font-semibold">{label}</p>
      <ParameterEvaluationItem
        label="Char edit distance"
        value={evaluation.distance.toFixed(0)}
        isExactMatch={isExactMatch}
      />
      <ParameterEvaluationItem
        label="Accuracy"
        value={`${(evaluation.estimatedAccuracy * 100).toFixed(1)}%`}
        isExactMatch={isExactMatch}
      />
    </div>
  );
};

const ParameterEvaluationItem = ({
  label,
  value,
  isExactMatch,
}: {
  label: string;
  value: string;
  isExactMatch: boolean;
}) => {
  return (
    <div className="flex items-center justify-between">
      <span className="text-xs text-muted-foreground">{label}</span>
      <span
        className={`text-sm font-medium ${
          isExactMatch ? "text-green-600" : "text-red-600"
        }`}
      >
        {value}
      </span>
    </div>
  );
};

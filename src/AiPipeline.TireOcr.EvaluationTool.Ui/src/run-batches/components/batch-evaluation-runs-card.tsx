import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/core/components/ui/card";
import { EvaluationRunsTable } from "@/runs/components/evaluation-runs-table";
import type { EvaluationRun } from "@/runs/dtos/get-evaluation-run-dto";

type BatchEvaluationRunsCardProps = {
  runs: EvaluationRun[];
};

export const BatchEvaluationRunsCard = ({
  runs,
}: BatchEvaluationRunsCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Evaluation Runs</CardTitle>
        <CardDescription>{runs.length} runs in this batch</CardDescription>
      </CardHeader>
      <CardContent>
        <EvaluationRunsTable runs={runs} />
      </CardContent>
    </Card>
  );
};

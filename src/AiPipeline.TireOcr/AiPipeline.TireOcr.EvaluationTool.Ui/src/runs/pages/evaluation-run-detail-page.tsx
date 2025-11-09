import { useParams } from "react-router-dom";
import { useRunDetailQuery } from "../queries/use-run-detail-query";
import { RunInfoCard } from "../components/run-info-card";
import { RunConfigCard } from "../../core/components/run-config-card";
import { RunDurationsCard } from "../components/run-durations-card";
import { RunEvaluationCard } from "../components/run-evaluation-card";
import { RunResultsCard } from "../components/run-results-card";
import ErrorFullpage from "@/core/components/placeholders/error-fullpage";
import SpinnerFullpage from "@/core/components/placeholders/spinner-fullpage";

const EvaluationRunDetailPage: React.FC = () => {
  const { runId } = useParams<{ runId: string }>();

  const { data: run, isLoading, error } = useRunDetailQuery(runId!);

  if (isLoading) return <SpinnerFullpage />;
  if (error) return <ErrorFullpage errorMessage="Failed to load run details" />;
  if (!run) return null;

  const estimatedCost = run.ocrResult?.estimatedCosts?.estimatedCost
    ? {
        amount: run.ocrResult.estimatedCosts.estimatedCost,
        currency: run.ocrResult.estimatedCosts.estimatedCostCurrency ?? ",-",
      }
    : null;

  return (
    <div className="container flex flex-col gap-y-6 py-8">
      <RunInfoCard evaluationRun={run} estimatedCost={estimatedCost} />

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <RunConfigCard runConfig={run.runConfig} />
        <RunDurationsCard run={run} />
      </div>

      {run.evaluation && <RunEvaluationCard evaluation={run.evaluation} />}

      <RunResultsCard run={run} />
    </div>
  );
};

export default EvaluationRunDetailPage;

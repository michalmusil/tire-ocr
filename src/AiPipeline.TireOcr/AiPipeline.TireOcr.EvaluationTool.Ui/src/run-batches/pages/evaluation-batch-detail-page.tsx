import { useParams } from "react-router-dom";
import { useRunBatchDetailQuery } from "../queries/use-run-batch-detail-query";
import { BatchInfoCard } from "../components/batch-info-card";
import { BatchEvaluationCountsCard } from "../components/batch-evaluation-counts-card";
import { BatchAccuracyCard } from "../components/batch-accuracy-card";
import { BatchEvaluationRunsCard } from "../components/batch-evaluation-runs-card";
import { BatchAverageTimesCard } from "../components/batch-average-times-card";
import SpinnerFullpage from "@/core/components/placeholders/spinner-fullpage";
import ErrorFullpage from "@/core/components/placeholders/error-fullpage";
import { RunConfigCard } from "@/core/components/run-config-card";

const EvaluationBatchDetailPage: React.FC = () => {
  const { batchId } = useParams<{ batchId: string }>();

  const {
    data: batchDetail,
    isLoading,
    error,
  } = useRunBatchDetailQuery(batchId!);

  if (isLoading) return <SpinnerFullpage />;
  if (error)
    return <ErrorFullpage errorMessage="Failed to load batch details" />;
  if (!batchDetail) return null;

  const totalCost = batchDetail.evaluationRuns.reduce(
    (total, run) => total + (run.ocrResult?.estimatedCosts?.estimatedCost ?? 0),
    0
  );
  const totalCostCurrency =
    batchDetail.evaluationRuns[0]?.ocrResult?.estimatedCosts
      ?.estimatedCostCurrency ?? ",-";

  return (
    <div className="container mx-auto py-8 space-y-6">
      <BatchInfoCard
        batch={batchDetail}
        totalCost={
          totalCost > 0
            ? { amount: totalCost, currency: totalCostCurrency }
            : null
        }
      />

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {batchDetail.batchConfig && (
          <RunConfigCard runConfig={batchDetail.batchConfig} />
        )}
        <BatchAverageTimesCard batchDetail={batchDetail} />
      </div>

      <BatchEvaluationCountsCard
        countsEvaluation={batchDetail.batchEvaluation.counts}
      />

      <BatchAccuracyCard
        distances={batchDetail.batchEvaluation.distances}
        averageCer={batchDetail.batchEvaluation.averageCer}
      />

      <BatchEvaluationRunsCard runs={batchDetail.evaluationRuns} />
    </div>
  );
};

export default EvaluationBatchDetailPage;

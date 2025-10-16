import { useParams } from "react-router-dom";
import { useRunBatchDetailQuery } from "../queries/use-run-batch-detail-query";
import SpinnerFullpage from "@/core/components/spinner-fullpage";
import ErrorFullpage from "@/core/components/error-fullpage";
import { BatchInfoCard } from "../components/batch-info-card";
import { BatchEvaluationCountsCard } from "../components/batch-evaluation-counts-card";
import { BatchAverageDistancesCard } from "../components/batch-average-distances-card";
import { BatchEvaluationRunsCard } from "../components/batch-evaluation-runs-card";

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

  return (
    <div className="container mx-auto py-8 space-y-6">
      <BatchInfoCard
        title={batchDetail.title}
        batchId={batchDetail.id}
        startedAt={batchDetail.startedAt}
        finishedAt={batchDetail.finishedAt}
      />

      <BatchEvaluationCountsCard
        countsEvaluation={batchDetail.batchEvaluation.counts}
      />

      <BatchAverageDistancesCard
        distances={batchDetail.batchEvaluation.distances}
      />

      <BatchEvaluationRunsCard runs={batchDetail.evaluationRuns} />
    </div>
  );
};

export default EvaluationBatchDetailPage;

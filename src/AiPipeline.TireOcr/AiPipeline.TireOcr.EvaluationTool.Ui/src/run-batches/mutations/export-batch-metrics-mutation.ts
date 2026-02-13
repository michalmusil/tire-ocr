import { useMutation } from "@tanstack/react-query";
import axios from "axios";

type ExportBatchMetricsProps = {
  batchId: string;
  inferenceStabilityRelativeBatchId?: string | null;
  annualFixedCostUsd?: number | null;
  expectedAnnualInferences?: number | null;
};

const exportBatchMetrics = async ({
  batchId,
  inferenceStabilityRelativeBatchId,
  annualFixedCostUsd,
  expectedAnnualInferences,
}: ExportBatchMetricsProps) => {
  const response = await axios.post(
    `/api/v1/Batch/${batchId}/ExportMetrics`,
    {
      inferenceStabilityRelativeBatchId,
      annualFixedCostUsd,
      expectedAnnualInferences,
    },
    {
      responseType: "blob",
    },
  );
  if (response.status !== 200) {
    throw new Error("Failed to export evaluation batch metrics");
  }

  return response.data;
};

export const useExportBatchMetricsMutation = () =>
  useMutation({ mutationFn: exportBatchMetrics });

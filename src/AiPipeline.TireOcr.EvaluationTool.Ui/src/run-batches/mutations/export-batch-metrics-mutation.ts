import { useMutation } from "@tanstack/react-query";
import axios from "axios";

type ExportBatchMetricsProps = {
  batchId: string;
  inferenceStabilityRelativeBatchIds?: string[] | null;
  fixedExpenditure?: number | null;
  calculateVariableExpenditure: boolean;
};

const exportBatchMetrics = async ({
  batchId,
  inferenceStabilityRelativeBatchIds,
  fixedExpenditure,
  calculateVariableExpenditure,
}: ExportBatchMetricsProps) => {
  const response = await axios.post(
    `/api/v1/Batch/${batchId}/ExportMetrics`,
    {
      inferenceStabilityRelativeBatchIds,
      fixedExpenditure,
      calculateVariableExpenditure,
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

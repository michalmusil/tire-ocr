import { useQuery } from "@tanstack/react-query";
import {
  RunBatchDetailResponseSchema,
  type RunBatchDetail,
} from "../dtos/get-run-batch-detail-dto";

const fetchRunBatchDetail = async (
  batchId: string
): Promise<RunBatchDetail> => {
  const response = await fetch(`/api/v1/Batch/${batchId}`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch run batch detail");
  }

  const json = await response.json();
  const parsed = RunBatchDetailResponseSchema.parse(json);
  return parsed.item;
};

export const useRunBatchDetailQuery = (batchId: string) => {
  return useQuery({
    queryKey: ["runBatchDetail", batchId],
    queryFn: () => fetchRunBatchDetail(batchId),
  });
};

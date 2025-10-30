import { useQuery } from "@tanstack/react-query";
import {
  RunBatchDetailResponseSchema,
  type RunBatchDetail,
} from "../dtos/get-run-batch-detail-dto";
import axios from "axios";

export const RunBatchDetailQueryKey = "runBatchDetail";

const fetchRunBatchDetail = async (
  batchId: string
): Promise<RunBatchDetail> => {
  const response = await axios.get(`/api/v1/Batch/${batchId}`);
  if (response.status !== 200) {
    throw new Error("Failed to fetch run batch detail");
  }

  const parsed = RunBatchDetailResponseSchema.parse(response.data);
  return parsed.item;
};

export const useRunBatchDetailQuery = (batchId: string) => {
  return useQuery({
    queryKey: [RunBatchDetailQueryKey, batchId],
    queryFn: () => fetchRunBatchDetail(batchId),
  });
};

import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import {
  PaginatedRunBatchesSchema,
  type PaginatedRunBatches,
} from "../dtos/get-run-batch-dto";

export const RunBatchesQueryKey = "runBatches";

const fetchRunBatches = async (
  page: number,
  pageSize: number,
  searchTerm: string | undefined | null
): Promise<PaginatedRunBatches> => {
  const response = await axios.get(
    `/api/v1/Batch?pageNumber=${page}&pageSize=${pageSize}&searchTerm=${
      searchTerm ?? ""
    }`
  );
  if (response.status !== 200) {
    throw new Error("Failed to fetch evaluation batches");
  }
  const parsed = PaginatedRunBatchesSchema.parse(response.data);
  return parsed;
};

export const useRunBatchesQuery = (
  page: number = 1,
  pageSize: number = 10,
  searchTerm: string | undefined | null = null
) => {
  return useQuery({
    queryKey: [RunBatchesQueryKey, page, pageSize, searchTerm],
    queryFn: () => fetchRunBatches(page, pageSize, searchTerm),
  });
};

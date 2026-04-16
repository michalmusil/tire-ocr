import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import {
  PaginatedEvaluationRunsSchema,
  type PaginatedEvaluationRuns,
} from "../dtos/get-evaluation-run-dto";

export const RunsQueryKey = "runs";

const fetchEvaluationRuns = async (
  page: number,
  pageSize: number,
  searchTerm: string | undefined | null
): Promise<PaginatedEvaluationRuns> => {
  const response = await axios.get(
    `/api/v1/Run?pageNumber=${page}&pageSize=${pageSize}&searchTerm=${
      searchTerm ?? ""
    }`
  );
  if (response.status !== 200) {
    throw new Error("Failed to fetch evaluation runs");
  }
  const parsed = PaginatedEvaluationRunsSchema.parse(response.data);
  return parsed;
};

export const useRunsQuery = (
  page: number = 1,
  pageSize: number = 10,
  searchTerm: string | undefined | null = null
) => {
  return useQuery({
    queryKey: [RunsQueryKey, page, pageSize, searchTerm],
    queryFn: () => fetchEvaluationRuns(page, pageSize, searchTerm),
  });
};

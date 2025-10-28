import { useQuery } from "@tanstack/react-query";
import {
  EvaluationRunSchema,
  type EvaluationRun,
} from "../dtos/get-evaluation-run-dto";
import axios from "axios";

const fetchRunDetail = async (runId: string): Promise<EvaluationRun> => {
  const response = await axios.get(`/api/v1/Run/${runId}`);
  if (response.status !== 200) {
    throw new Error("Failed to fetch run detail");
  }

  const parsed = EvaluationRunSchema.parse(response.data["item"]);
  return parsed;
};

export const useRunDetailQuery = (runId: string) => {
  return useQuery({
    queryKey: ["runDetail", runId],
    queryFn: () => fetchRunDetail(runId),
  });
};

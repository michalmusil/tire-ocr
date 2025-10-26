import { useQuery } from "@tanstack/react-query";
import {
  EvaluationRunSchema,
  type EvaluationRun,
} from "../dtos/get-evaluation-run-dto";

const fetchRunDetail = async (runId: string): Promise<EvaluationRun> => {
  const response = await fetch(`/api/v1/Run/${runId}`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch run detail");
  }

  const json = await response.json();
  const parsed = EvaluationRunSchema.parse(json["item"]);
  return parsed;
};

export const useRunDetailQuery = (runId: string) => {
  return useQuery({
    queryKey: ["runDetail", runId],
    queryFn: () => fetchRunDetail(runId),
  });
};

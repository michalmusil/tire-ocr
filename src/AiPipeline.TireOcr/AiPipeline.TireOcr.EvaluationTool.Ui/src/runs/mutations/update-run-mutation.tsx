import { useMutation } from "@tanstack/react-query";
import axios from "axios";

type UpdateRunProps = {
  runId: string;
  newTitle?: string | null;
};

const updateRun = async ({ runId, newTitle }: UpdateRunProps) => {
  const request = {
    runTitle: newTitle,
  };
  const response = await axios.put(`/api/v1/Run/${runId}`, request);
  if (response.status !== 200) {
    throw new Error(`Failed to update evaluation run ${runId}`);
  }
};

export const useUpdateRunMutation = () =>
  useMutation({ mutationFn: updateRun });

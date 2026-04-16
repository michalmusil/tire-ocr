import { useMutation } from "@tanstack/react-query";
import axios from "axios";

type UpdateRunProps = {
  runId: string;
  title?: string | null;
  description?: string | null;
};

const updateRun = async ({ runId, title, description }: UpdateRunProps) => {
  const request = {
    title: title,
    description: description,
  };
  const response = await axios.put(`/api/v1/Run/${runId}`, request);
  if (response.status !== 200) {
    throw new Error(`Failed to update evaluation run ${runId}`);
  }
};

export const useUpdateRunMutation = () =>
  useMutation({ mutationFn: updateRun });
